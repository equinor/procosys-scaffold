﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PCS5.Domain.Tests.AggregateModels.LinkAggregate;

[TestClass]
public class LinkTests
{
    private Link _dut;
    private readonly string _title = "Pro A";
    private readonly Guid _sourceGuid = Guid.NewGuid();
    private readonly string _url = "Desc A";

    [TestInitialize]
    public void Setup() => _dut = new Link(_sourceGuid, _title, _url);

    [TestMethod]
    public void Constructor_ShouldSetProperties()
    {
        Assert.AreEqual(_title, _dut.Title);
        Assert.AreEqual(_url, _dut.Url);
        Assert.AreEqual(_sourceGuid, _dut.SourceGuid);
        Assert.AreNotEqual(_sourceGuid, _dut.Guid);
        Assert.AreNotEqual(Guid.Empty, _dut.Guid);
    }
}
