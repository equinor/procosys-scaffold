﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.PCS5.Infrastructure.Repositories;
using Equinor.ProCoSys.PCS5.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;

namespace Equinor.ProCoSys.PCS5.Infrastructure.Tests.Repositories;

[TestClass]
public class ProjectRepositoryTests : EntityWithGuidRepositoryTestBase<Project>
{
    private new ProjectRepository _dut;
    private readonly string _knownProjectName = "ProjectName";

    protected override void SetupRepositoryWithOneKnownItem()
    {
        var project = new Project(TestPlant, Guid.NewGuid(), _knownProjectName, "Description of project");
        _knownGuid = project.Guid;
        project.SetProtectedIdForTesting(_knownId);

        var projects = new List<Project> { project };

        _dbSetMock = projects.AsQueryable().BuildMockDbSet();

        _contextHelper
            .ContextMock
            .Setup(x => x.Projects)
            .Returns(_dbSetMock.Object);

        _dut = new ProjectRepository(_contextHelper.ContextMock.Object);
        base._dut = _dut;
    }

    protected override Project GetNewEntity() => new(TestPlant, Guid.NewGuid(), "New Project", "D");

    [TestMethod]
    public async Task GetProjectOnlyByName_KnownName_ShouldReturnProject()
    {
        var result = await _dut.TryGetProjectByNameAsync(_knownProjectName);

        Assert.IsNotNull(result);
        Assert.AreEqual(_knownProjectName, result.Name);
    }

    [TestMethod]
    public async Task GetProjectOnlyByName_UnknownName_ShouldReturnNull()
    {
        var result = await _dut.TryGetProjectByNameAsync(Guid.NewGuid().ToString());

        Assert.IsNull(result);
    }
}
