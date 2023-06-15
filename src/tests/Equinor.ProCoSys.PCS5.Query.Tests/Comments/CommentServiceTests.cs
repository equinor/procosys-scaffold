﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.CommentAggregate;
using Equinor.ProCoSys.PCS5.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Equinor.ProCoSys.PCS5.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Equinor.ProCoSys.PCS5.Query.Comments;

namespace Equinor.ProCoSys.PCS5.Query.Tests.Comments;

[TestClass]
public class CommentServiceTests : ReadOnlyTestsBase
{
    private Comment _createdComment;
    private Guid _sourceGuid;

    protected override void SetupNewDatabase(DbContextOptions<PCS5Context> dbContextOptions)
    {
        using var context = new PCS5Context(dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);

        _sourceGuid = Guid.NewGuid();
        _createdComment = new Comment("X", _sourceGuid, "T");

        context.Comments.Add(_createdComment);
        context.SaveChangesAsync().Wait();
    }

    [TestMethod]
    public async Task GetAllForSourceAsync_ShouldReturnCorrectDtos()
    {
        // Arrange
        await using var context = new PCS5Context(_dbContextOptions, _plantProvider, _eventDispatcher, _currentUserProvider);
        var dut = new CommentService(context);

        // Act
        var result = await dut.GetAllForSourceAsync(_sourceGuid, default);

        // Assert
        var commentDtos = result.ToList();
        Assert.AreEqual(1, commentDtos.Count);
        var commentDto = commentDtos.ElementAt(0);
        Assert.AreEqual(_createdComment.SourceGuid, commentDto.SourceGuid);
        Assert.AreEqual(_createdComment.Guid, commentDto.Guid);
        Assert.AreEqual(_createdComment.Text, commentDto.Text);
        var createdBy = commentDto.CreatedBy;
        Assert.IsNotNull(createdBy);
        Assert.AreEqual(CurrentUserOid, createdBy.Guid);
        Assert.AreEqual(_createdComment.CreatedAtUtc, commentDto.CreatedAtUtc);
    }
}
