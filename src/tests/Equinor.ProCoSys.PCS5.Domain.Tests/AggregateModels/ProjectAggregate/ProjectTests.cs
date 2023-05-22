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
}
