﻿using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.PCS5.Domain;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.FooAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.PCS5.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using IDomainMarker = Equinor.ProCoSys.PCS5.Domain.IDomainMarker;

namespace Equinor.ProCoSys.PCS5.Infrastructure;

public class PCS5Context : DbContext, IUnitOfWork, IReadOnlyContext
{
    public static string PCS5ContextConnectionStringName = "PCS5Context";

    private readonly IPlantProvider _plantProvider;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ICurrentUserProvider _currentUserProvider;

    public PCS5Context(
        DbContextOptions<PCS5Context> options,
        IPlantProvider plantProvider,
        IEventDispatcher eventDispatcher,
        ICurrentUserProvider currentUserProvider)
        : base(options)
    {
        _plantProvider = plantProvider;
        _eventDispatcher = eventDispatcher;
        _currentUserProvider = currentUserProvider;
    }
       
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (DebugOptions.DebugEntityFrameworkInDevelopment)
        {
            optionsBuilder.LogTo(Console.WriteLine);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        SetGlobalPlantFilter(modelBuilder);
    }      

    public static DateTimeKindConverter DateTimeKindConverter { get; } = new();

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Foo> Foos => Set<Foo>();
    public DbSet<Project> Projects => Set<Project>();

    private void SetGlobalPlantFilter(ModelBuilder modelBuilder)
    {
        // Set global query filter on entities inheriting from PlantEntityBase
        // https://gunnarpeipman.com/ef-core-global-query-filters/
        foreach (var type in TypeProvider.GetEntityTypes(typeof(IDomainMarker).GetTypeInfo().Assembly, typeof(PlantEntityBase)))
        {
            typeof(PCS5Context)
                .GetMethod(nameof(PCS5Context.SetGlobalQueryFilter))
                ?.MakeGenericMethod(type)
                .Invoke(this, new object[] { modelBuilder });
        }
    }

    public void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : PlantEntityBase =>
        builder
            .Entity<T>()
            .HasQueryFilter(e => e.Plant == _plantProvider.Plant);

    public IQueryable<TEntity> QuerySet<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchPreSaveEventsAsync(cancellationToken);
        await SetAuditDataAsync();
        UpdateConcurrencyToken();
            
        try
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchPostSaveEventsAsync(cancellationToken);
            return result;
        }
        catch (DbUpdateConcurrencyException concurrencyException)
        {
            throw new ConcurrencyException("Data store operation failed. Data may have been modified or deleted since entities were loaded.", concurrencyException);
        }
    }
            
    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken = default) 
        => await base.Database.BeginTransactionAsync(cancellationToken);

    public void Commit() => base.Database.CommitTransaction();

    private void UpdateConcurrencyToken()
    {
        var modifiedEntries = ChangeTracker
            .Entries<EntityBase>()
            .Where(x => x.State == EntityState.Modified || x.State == EntityState.Deleted);

        foreach (var entry in modifiedEntries)
        {
            var currentRowVersion = entry.CurrentValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
            var originalRowVersion = entry.OriginalValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
            for (var i = 0; i < currentRowVersion.Length; i++)
            {
                originalRowVersion[i] = currentRowVersion[i];
            }
        }
    }

    private async Task DispatchPreSaveEventsAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
            .Entries<EntityBase>()
            .Where(x => x.Entity.PreSaveDomainEvents != null && x.Entity.PreSaveDomainEvents.Any())
            .Select(x => x.Entity);
        await _eventDispatcher.DispatchPreSaveAsync(entities, cancellationToken);
    }

    private async Task DispatchPostSaveEventsAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
            .Entries<EntityBase>()
            .Where(x => x.Entity.PostSaveDomainEvents != null && x.Entity.PostSaveDomainEvents.Any())
            .Select(x => x.Entity);
        await _eventDispatcher.DispatchPostSaveAsync(entities, cancellationToken);
    }

    private async Task SetAuditDataAsync()
    {
        var addedEntries = ChangeTracker
            .Entries<ICreationAuditable>()
            .Where(x => x.State == EntityState.Added)
            .ToList();
        var modifiedEntries = ChangeTracker
            .Entries<IModificationAuditable>()
            .Where(x => x.State == EntityState.Modified)
            .ToList();

        if (addedEntries.Any() || modifiedEntries.Any())
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();
            var currentUser = await Persons.SingleOrDefaultAsync(p => p.Oid == currentUserOid);
            if (currentUser == null)
            {
                throw new Exception(
                    $"{nameof(Person)} {currentUserOid} not found when setting SetCreated / SetModified");
            }

            foreach (var entry in addedEntries)
            {
                entry.Entity.SetCreated(currentUser);
            }

            foreach (var entry in modifiedEntries)
            {
                entry.Entity.SetModified(currentUser);
            }
        }
    }
}
