﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooComments;
using Equinor.ProCoSys.PCS5.Query.Comments;
using Equinor.ProCoSys.PCS5.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.Query.Tests.GetFooComments;

[TestClass]
public class GetFooCommentsQueryHandlerTests : TestsBase
{
    private GetFooCommentsQueryHandler _dut;
    private Mock<ICommentService> _commentServiceMock;
    private GetFooCommentsQuery _query;
    private CommentDto _commentDto;

    [TestInitialize]
    public void Setup()
    {
        _query = new GetFooCommentsQuery(Guid.NewGuid());

        _commentDto = new CommentDto(
            _query.FooGuid,
            Guid.NewGuid(), 
            "T", 
            new PersonDto(Guid.NewGuid(), "First", "Last", "UN", "Email"),
            new DateTime(2023, 6, 11, 1, 2, 3));
        var commentDtos = new List<CommentDto>
        {
            _commentDto
        };
        _commentServiceMock = new Mock<ICommentService>();
        _commentServiceMock.Setup(l => l.GetAllForSourceAsync(_query.FooGuid, default))
            .ReturnsAsync(commentDtos);

        _dut = new GetFooCommentsQueryHandler(_commentServiceMock.Object);
    }

    [TestMethod]
    public async Task HandlingQuery_ShouldReturn_Comments()
    {
        // Act
        var result = await _dut.Handle(_query, default);

        // Assert
        Assert.IsInstanceOfType(result.Data, typeof(IEnumerable<CommentDto>));
        var comment = result.Data.Single();
        Assert.AreEqual(_commentDto.SourceGuid, comment.SourceGuid);
        Assert.AreEqual(_commentDto.Guid, comment.Guid);
        Assert.AreEqual(_commentDto.CreatedAtUtc, comment.CreatedAtUtc);
        var createdBy = comment.CreatedBy;
        Assert.IsNotNull(createdBy);
        Assert.AreEqual(_commentDto.CreatedBy.Guid, createdBy.Guid);
        Assert.AreEqual(_commentDto.CreatedBy.FirstName, createdBy.FirstName);
        Assert.AreEqual(_commentDto.CreatedBy.LastName, createdBy.LastName);
        Assert.AreEqual(_commentDto.CreatedBy.UserName, createdBy.UserName);
        Assert.AreEqual(_commentDto.CreatedBy.Email, createdBy.Email);
        Assert.AreEqual(_commentDto.CreatedAtUtc, comment.CreatedAtUtc);
    }

    [TestMethod]
    public async Task HandlingQuery_Should_CallGetAllForSource_OnCommentService()
    {
        // Act
        await _dut.Handle(_query, default);

        // Assert
        _commentServiceMock.Verify(u => u.GetAllForSourceAsync(
            _query.FooGuid,
            default), Times.Exactly(1));
    }
}
