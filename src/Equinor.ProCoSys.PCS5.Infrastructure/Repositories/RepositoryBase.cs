﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.PCS5.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : Domain.IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
{
    protected readonly PCS5Context Context;
    protected readonly DbSet<TEntity> Set;
    protected readonly IQueryable<TEntity> DefaultQuery;

    protected RepositoryBase(PCS5Context context, DbSet<TEntity> set)
        : this(context, set, set)
    {
    }

    protected RepositoryBase(PCS5Context context, DbSet<TEntity> set, IQueryable<TEntity> defaultQuery)
    {
        Context = context;
        Set = set;
        DefaultQuery = defaultQuery;
    }

    public virtual void Add(TEntity entity) =>
        Set.Add(entity);

    public Task<bool> Exists(int id) =>
        DefaultQuery.AnyAsync(x => x.Id == id);

    public virtual Task<List<TEntity>> GetAllAsync() =>
        DefaultQuery.ToListAsync();

    public virtual Task<TEntity?> GetByIdAsync(int id) =>
        DefaultQuery.SingleOrDefaultAsync(x => x.Id == id);

    public virtual Task<TEntity?> GetByGuidAsync(Guid guid) =>
        DefaultQuery.SingleOrDefaultAsync(x => x.Guid == guid);

    public Task<List<TEntity>> GetByIdsAsync(IEnumerable<int> ids) =>
        DefaultQuery.Where(x => ids.Contains(x.Id)).ToListAsync();

    public virtual void Remove(TEntity entity)
    {
        if (entity is IVoidable voidable)
        {
            if (!voidable.IsVoided)
            {
                throw new Exception($"{nameof(entity)} must be voided before delete");
            }
        }
        Set.Remove(entity);
    }
}
