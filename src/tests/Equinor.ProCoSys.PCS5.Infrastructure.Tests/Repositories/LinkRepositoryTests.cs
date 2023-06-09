﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Equinor.ProCoSys.PCS5.Infrastructure.Repositories;
using Equinor.ProCoSys.PCS5.Test.Common.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;

namespace Equinor.ProCoSys.PCS5.Infrastructure.Tests.Repositories;

[TestClass]
public class LinkRepositoryTests : EntityWithGuidRepositoryTestBase<Link>
{
    protected override void SetupRepositoryWithOneKnownItem()
    {
        var link = new Link("Whatever", Guid.NewGuid(), "T", "www");
        _knownGuid = link.Guid;
        link.SetProtectedIdForTesting(_knownId);
        var links = new List<Link> { link };

        _dbSetMock = links.AsQueryable().BuildMockDbSet();

        _contextHelper
            .ContextMock
            .Setup(x => x.Links)
            .Returns(_dbSetMock.Object);

        _dut = new LinkRepository(_contextHelper.ContextMock.Object);
    }

    protected override Link GetNewEntity() => new("Whatever", Guid.NewGuid(), "New link", "U");
}
