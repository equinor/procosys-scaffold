﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Equinor.ProCoSys.PCS5.Domain.Audit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PCS5.Domain.Tests.AggregateModels.LinkAggregate;

[TestClass]
public class LinkTests : IModificationAuditableTests
{
    private Link _dut;
    private readonly string _sourceType = "X";
    private readonly Guid _sourceGuid = Guid.NewGuid();
    private readonly string _title = "A";
    private readonly string _url = "Desc A";

    protected override ICreationAuditable GetCreationAuditable() => _dut;
    protected override IModificationAuditable GetModificationAuditable() => _dut;

    [TestInitialize]
    public void Setup() => _dut = new Link(_sourceType, _sourceGuid, _title, _url);

    [TestMethod]
    public void Constructor_ShouldSetProperties()
    {
        Assert.AreEqual(_title, _dut.Title);
        Assert.AreEqual(_url, _dut.Url);
        Assert.AreEqual(_sourceType, _dut.SourceType);
        Assert.AreEqual(_sourceGuid, _dut.SourceGuid);
        Assert.AreNotEqual(_sourceGuid, _dut.Guid);
        Assert.AreNotEqual(Guid.Empty, _dut.Guid);
    }
}
