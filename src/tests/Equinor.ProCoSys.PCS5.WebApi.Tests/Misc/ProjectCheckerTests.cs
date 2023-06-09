﻿using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.PCS5.WebApi.Misc;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Equinor.ProCoSys.PCS5.Command;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Misc;

[TestClass]
public class ProjectCheckerTests
{
    private readonly Guid _currentUserOid = new("12345678-1234-1234-1234-123456789123");
    private readonly string _plant = "Plant";
    private readonly string _project = "Project";

    private Mock<IPlantProvider> _plantProviderMock;
    private Mock<ICurrentUserProvider> _currentUserProviderMock;
    private Mock<IPermissionCache> _permissionCacheMock;
    private TestRequest _testRequest;
    private ProjectChecker _dut;

    [TestInitialize]
    public void Setup()
    {
        _plantProviderMock = new Mock<IPlantProvider>();
        _plantProviderMock.SetupGet(p => p.Plant).Returns(_plant);

        _currentUserProviderMock = new Mock<ICurrentUserProvider>();
        _currentUserProviderMock.Setup(c => c.GetCurrentUserOid()).Returns(_currentUserOid);

        _permissionCacheMock = new Mock<IPermissionCache>();

        _testRequest = new TestRequest(_project);
        _dut = new ProjectChecker(_plantProviderMock.Object, _currentUserProviderMock.Object, _permissionCacheMock.Object);
    }

    [TestMethod]
    public async Task EnsureValidProjectAsync_ShouldValidateOK()
    {
        // Arrange
        _permissionCacheMock.Setup(p => p.IsAValidProjectForUserAsync(_plant, _currentUserOid, _project)).ReturnsAsync(true);

        // Act
        await _dut.EnsureValidProjectAsync(_testRequest);
    }

    [TestMethod]
    public async Task EnsureValidProjectAsync_ShouldThrowInvalidException_WhenProjectIsNotValid()
    {
        // Arrange
        _permissionCacheMock.Setup(p => p.IsAValidProjectForUserAsync(_plant, _currentUserOid, _project)).ReturnsAsync(false);

        // Act
        await Assert.ThrowsExceptionAsync<InValidProjectException>(() => _dut.EnsureValidProjectAsync(_testRequest));
    }

    [TestMethod]
    public async Task EnsureValidProjectAsync_ShouldThrowException_WhenRequestIsNull()
        => await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _dut.EnsureValidProjectAsync((IBaseRequest)null));

    private class TestRequest : IIsProjectCommand
    {
        public TestRequest(string projectName) => ProjectName = projectName;

        public string ProjectName { get; }
    }
}
