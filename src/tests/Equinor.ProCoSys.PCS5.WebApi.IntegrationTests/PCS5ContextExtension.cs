﻿using System;
using System.Linq;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.AttachmentAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.CommentAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.FooAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.PCS5.Infrastructure;
using Equinor.ProCoSys.PCS5.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.PCS5.WebApi.IntegrationTests;

public static class PCS5ContextExtension
{
    private static string _seederOid = "00000000-0000-0000-0000-999999999999";

    public static void CreateNewDatabaseWithCorrectSchema(this PCS5Context dbContext)
    {
        var migrations = dbContext.Database.GetPendingMigrations();
        if (migrations.Any())
        {
            dbContext.Database.Migrate();
        }
    }

    public static void Seed(this PCS5Context dbContext, IServiceProvider serviceProvider, KnownTestData knownTestData)
    {
        var userProvider = serviceProvider.GetRequiredService<CurrentUserProvider>();
        var plantProvider = serviceProvider.GetRequiredService<PlantProvider>();
        userProvider.SetCurrentUserOid(new Guid(_seederOid));
        plantProvider.SetPlant(knownTestData.Plant);
            
        /* 
         * Add the initial seeder user. Don't do this through the UnitOfWork as this expects/requires the current user to exist in the database.
         * This is the first user that is added to the database and will not get "Created" and "CreatedBy" data.
         */
        EnsureCurrentUserIsSeeded(dbContext, userProvider);

        var plant = plantProvider.Plant;
            
        var project = SeedProject(
            dbContext,
            plant,
            KnownTestData.ProjectGuidA,
            KnownTestData.ProjectNameA,
            KnownTestData.ProjectDescriptionA);
        var foo = SeedFoo(dbContext, plant, project, KnownTestData.FooA);
        knownTestData.FooAGuid = foo.Guid;

        project = SeedProject(
            dbContext, 
            plant, 
            KnownTestData.ProjectGuidB,
            KnownTestData.ProjectNameB, 
            KnownTestData.ProjectDescriptionB);
        foo = SeedFoo(dbContext, plant, project, KnownTestData.FooB);
        knownTestData.FooBGuid = foo.Guid;

        var link = SeedLink(dbContext, "Foo", foo.Guid, "VG", "www.vg.no");
        knownTestData.LinkInFooAGuid = link.Guid;

        var comment = SeedComment(dbContext, "Foo", foo.Guid, "Comment");
        knownTestData.CommentInFooAGuid = comment.Guid;

        var attachment = SeedAttachment(dbContext, plant, "Foo", foo.Guid, "fil.txt");
        knownTestData.AttachmentInFooAGuid = attachment.Guid;
    }

    private static void EnsureCurrentUserIsSeeded(PCS5Context dbContext, ICurrentUserProvider userProvider)
    {
        var personRepository = new PersonRepository(dbContext);
        var seeder = personRepository.TryGetByGuidAsync(userProvider.GetCurrentUserOid()).Result;
        if (seeder == null)
        {
            SeedCurrentUserAsPerson(dbContext, userProvider);
        }
    }

    private static void SeedCurrentUserAsPerson(PCS5Context dbContext, ICurrentUserProvider userProvider)
    {
        var personRepository = new PersonRepository(dbContext);
        var person = new Person(userProvider.GetCurrentUserOid(), "Siri", "Seed", "SSEED", "siri@pcs.com");
        personRepository.Add(person);
        dbContext.SaveChangesAsync().Wait();
    }

    private static Project SeedProject(
        PCS5Context dbContext,
        string plant,
        Guid guid,
        string name,
        string desc)
    {
        var projectRepository = new ProjectRepository(dbContext);
        var project = new Project(plant, guid, name, desc);
        projectRepository.Add(project);
        dbContext.SaveChangesAsync().Wait();
        return project;
    }

    private static Foo SeedFoo(PCS5Context dbContext, string plant, Project project, string title)
    {
        var fooRepository = new FooRepository(dbContext);
        var foo = new Foo(plant, project, title);
        fooRepository.Add(foo);
        dbContext.SaveChangesAsync().Wait();
        return foo;
    }

    private static Link SeedLink(PCS5Context dbContext, string sourceType, Guid sourceGuid, string title, string url)
    {
        var linkRepository = new LinkRepository(dbContext);
        var link = new Link(sourceType, sourceGuid, title, url);
        linkRepository.Add(link);
        dbContext.SaveChangesAsync().Wait();
        return link;
    }

    private static Comment SeedComment(PCS5Context dbContext, string sourceType, Guid sourceGuid, string text)
    {
        var commentRepository = new CommentRepository(dbContext);
        var comment = new Comment(sourceType, sourceGuid, text);
        commentRepository.Add(comment);
        dbContext.SaveChangesAsync().Wait();
        return comment;
    }

    private static Attachment SeedAttachment(
        PCS5Context dbContext,
        string plant,
        string sourceType,
        Guid sourceGuid,
        string fileName)
    {
        var attachmentRepository = new AttachmentRepository(dbContext);
        var attachment = new Attachment(sourceType, sourceGuid, plant, fileName);
        attachmentRepository.Add(attachment);
        dbContext.SaveChangesAsync().Wait();
        return attachment;
    }
}
