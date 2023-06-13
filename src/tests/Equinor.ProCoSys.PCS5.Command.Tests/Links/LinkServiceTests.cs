﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.PCS5.Command.Links;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Equinor.ProCoSys.PCS5.Domain.Events.DomainEvents.LinkEvents;
using Equinor.ProCoSys.PCS5.Test.Common;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.Command.Tests.Links;

[TestClass]
public class LinkServiceTests : TestsBase
{
    private readonly string _rowVersion = "AAAAAAAAABA=";
    private readonly Guid _sourceGuid = Guid.NewGuid();
    private readonly Guid _linkGuid = Guid.NewGuid();
    private Mock<ILinkRepository> _linkRepositoryMock;
    private LinkService _dut;
    private Link _linkAddedToRepository;
    private Link _existingLink;

    [TestInitialize]
    public void Setup()
    {
        _linkRepositoryMock = new Mock<ILinkRepository>();
        _linkRepositoryMock
            .Setup(x => x.Add(It.IsAny<Link>()))
            .Callback<Link>(link =>
            {
                _linkAddedToRepository = link;
            });
        _existingLink = new Link("Whatever", _sourceGuid, "T", "www");
        _linkRepositoryMock.Setup(l => l.TryGetByGuidAsync(_linkGuid))
            .ReturnsAsync(_existingLink);

        _dut = new LinkService(
            _linkRepositoryMock.Object,
            _unitOfWorkMock.Object,
            new Mock<ILogger<LinkService>>().Object);
    }

    #region AddAsync
    [TestMethod]
    public async Task AddAsync_ShouldAddLinkToRepository()
    {
        // Arrange 
        var sourceType = "Whatever";
        var title = "T";
        var url = "U";

        // Act
        await _dut.AddAsync(sourceType, _sourceGuid, title, url, default);

        // Assert
        Assert.IsNotNull(_linkAddedToRepository);
        Assert.AreEqual(_sourceGuid, _linkAddedToRepository.SourceGuid);
        Assert.AreEqual(sourceType, _linkAddedToRepository.SourceType);
        Assert.AreEqual(title, _linkAddedToRepository.Title);
        Assert.AreEqual(url, _linkAddedToRepository.Url);
    }

    [TestMethod]
    public async Task AddAsync_ShouldSaveOnce()
    {
        // Act
        await _dut.AddAsync("Whatever", _sourceGuid, "T", "www", default);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddLinkCreatedEvent()
    {
        // Act
        await _dut.AddAsync("Whatever", _sourceGuid, "T", "www", default);

        // Assert
        Assert.IsInstanceOfType(_linkAddedToRepository.DomainEvents.First(), typeof(LinkCreatedEvent));
    }
    #endregion

    #region ExistsAsync
    [TestMethod]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKnownLink()
    {
        // Act
        var result = await _dut.ExistsAsync(_linkGuid);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ExistsAsync_ShouldReturnNull_WhenUnknownLink()
    {
        // Arrange
        _linkRepositoryMock.Setup(l => l.TryGetByGuidAsync(_linkGuid))
            .ReturnsAsync((Link)null);

        // Act
        var result = await _dut.ExistsAsync(_linkGuid);

        // Assert
        Assert.IsFalse(result);
    }
    #endregion

    #region UpdateAsync
    [TestMethod]
    public async Task UpdateAsync_ShouldUpdateLink_WhenKnownLink()
    {
        // Arrange 
        var title = "newT";
        var url = "newU";

        // Act
        await _dut.UpdateAsync( _linkGuid, title, url, _rowVersion, default);

        // Assert
        Assert.AreEqual(url, _existingLink.Url);
        Assert.AreEqual(title, _existingLink.Title);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldThrowException_WhenUnknownLink()
    {
        // Arrange
        _linkRepositoryMock.Setup(l => l.TryGetByGuidAsync(_linkGuid))
            .ReturnsAsync((Link)null);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<Exception>(()
            => _dut.UpdateAsync(_linkGuid, "T", "www", _rowVersion, default));
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldSaveOnce()
    {
        // Act
        await _dut.UpdateAsync(_linkGuid, "T", "www", _rowVersion, default);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldAddLinkUpdatedEvent()
    {
        // Act
        await _dut.UpdateAsync(_linkGuid, "T", "www", _rowVersion, default);

        // Assert
        Assert.IsInstanceOfType(_existingLink.DomainEvents.Last(), typeof(LinkUpdatedEvent));
    }

    [TestMethod]
    public async Task UpdateAsync_ShouldSetAndReturnRowVersion()
    {
        // Act
        var result = await _dut.UpdateAsync(_linkGuid, "T", "www", _rowVersion, default);

        // Assert
        // In real life EF Core will create a new RowVersion when save.
        // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
        Assert.AreEqual(_rowVersion, result);
        Assert.AreEqual(_rowVersion, _existingLink.RowVersion.ConvertToString());
    }
    #endregion

    #region DeleteAsync
    [TestMethod]
    public async Task DeleteAsync_ShouldDeleteLink_WhenKnownLink()
    {
        // Act
        await _dut.DeleteAsync(_linkGuid, _rowVersion, default);

        // Assert
        _linkRepositoryMock.Verify(r => r.Remove(_existingLink), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldThrowException_WhenUnknownLink()
    {
        // Arrange
        _linkRepositoryMock.Setup(l => l.TryGetByGuidAsync(_linkGuid))
            .ReturnsAsync((Link)null);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<Exception>(()
            => _dut.DeleteAsync(_linkGuid, _rowVersion, default));
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldSaveOnce()
    {
        // Act
        await _dut.DeleteAsync(_linkGuid,_rowVersion, default);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldAddLinkDeletedEvent()
    {
        // Act
        await _dut.DeleteAsync(_linkGuid, _rowVersion, default);

        // Assert
        Assert.IsInstanceOfType(_existingLink.DomainEvents.Last(), typeof(LinkDeletedEvent));
    }

    [TestMethod]
    public async Task DeleteAsync_ShouldSetAndReturnRowVersion()
    {
        // Act
        await _dut.DeleteAsync(_linkGuid, _rowVersion, default);

        // Assert
        // In real life EF Core will create a new RowVersion when save.
        // Since UnitOfWorkMock is a Mock this will not happen here, so we assert that RowVersion is set from command
        Assert.AreEqual(_rowVersion, _existingLink.RowVersion.ConvertToString());
    }
    #endregion
}
