﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PCS5.Domain.Tests.AggregateModels.ProjectAggregate;

[TestClass]
public class ProjectTests
{
    private Project _dut;
    private readonly string _testPlant = "PlantA";
    private readonly string _name = "Pro A";
    private readonly Guid _guid = Guid.NewGuid();
    private readonly string _description = "Desc A";

    [TestInitialize]
    public void Setup() => _dut = new Project(_testPlant, _guid, _name, _description);

    [TestMethod]
    public void Constructor_ShouldSetProperties()
    {
        Assert.AreEqual(_testPlant, _dut.Plant);
        Assert.AreEqual(_name, _dut.Name);
        Assert.AreEqual(_description, _dut.Description);
        Assert.AreEqual(_guid, _dut.Guid);
    }

    #region DeleteInSource

    [TestMethod]
    public void DeleteInSource_ShouldAlsoClose()
    {
        // Arrange
        Assert.IsFalse(_dut.IsDeletedInSource);
        Assert.IsFalse(_dut.IsClosed);

        // Act
        _dut.IsDeletedInSource = true;

        // Assert
        Assert.IsTrue(_dut.IsClosed);
    }

    [TestMethod]
    public void UnDeleteInSource_ShouldThrowException()
    {
        // Arrange
        _dut.IsDeletedInSource = true;
        Assert.IsTrue(_dut.IsDeletedInSource);

        // Act and Assert
        Assert.ThrowsException<Exception>(() => _dut.IsDeletedInSource = false);
    }

    #endregion
}
