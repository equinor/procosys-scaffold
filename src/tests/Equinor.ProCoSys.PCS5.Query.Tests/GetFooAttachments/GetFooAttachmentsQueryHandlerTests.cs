﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Query.FooQueries.GetFooAttachments;
using Equinor.ProCoSys.PCS5.Query.Attachments;
using Equinor.ProCoSys.PCS5.Test.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.Query.Tests.GetFooAttachments;

[TestClass]
public class GetFooAttachmentsQueryHandlerTests : TestsBase
{
    private GetFooAttachmentsQueryHandler _dut;
    private Mock<IAttachmentService> _attachmentServiceMock;
    private GetFooAttachmentsQuery _query;
    private AttachmentDto _attachmentDto;

    [TestInitialize]
    public void Setup()
    {
        _query = new GetFooAttachmentsQuery(Guid.NewGuid());

        _attachmentDto = new AttachmentDto(
            _query.FooGuid,
            Guid.NewGuid(), 
            "F.txt",
            new PersonDto(Guid.NewGuid(), "First1", "Last1", "UN1", "Email1"),
            new DateTime(2023, 6, 11, 1, 2, 3),
            new PersonDto(Guid.NewGuid(), "First2", "Last2", "UN2", "Email2"),
            new DateTime(2023, 6, 12, 2, 3, 4),
            "R");
        var attachmentDtos = new List<AttachmentDto>
        {
            _attachmentDto
        };
        _attachmentServiceMock = new Mock<IAttachmentService>();
        _attachmentServiceMock.Setup(l => l.GetAllForSourceAsync(_query.FooGuid, default))
            .ReturnsAsync(attachmentDtos);

        _dut = new GetFooAttachmentsQueryHandler(_attachmentServiceMock.Object);
    }

    [TestMethod]
    public async Task HandlingQuery_ShouldReturn_Attachments()
    {
        // Act
        var result = await _dut.Handle(_query, default);

        // Assert
        Assert.IsInstanceOfType(result.Data, typeof(IEnumerable<AttachmentDto>));
        var attachment = result.Data.Single();
        Assert.AreEqual(_attachmentDto.SourceGuid, attachment.SourceGuid);
        Assert.AreEqual(_attachmentDto.Guid, attachment.Guid);
        Assert.AreEqual(_attachmentDto.FileName, attachment.FileName);
        Assert.AreEqual(_attachmentDto.RowVersion, attachment.RowVersion);
        
        var createdBy = attachment.CreatedBy;
        Assert.IsNotNull(createdBy);
        Assert.AreEqual(_attachmentDto.CreatedBy.Guid, createdBy.Guid);
        Assert.AreEqual(_attachmentDto.CreatedBy.FirstName, createdBy.FirstName);
        Assert.AreEqual(_attachmentDto.CreatedBy.LastName, createdBy.LastName);
        Assert.AreEqual(_attachmentDto.CreatedBy.UserName, createdBy.UserName);
        Assert.AreEqual(_attachmentDto.CreatedBy.Email, createdBy.Email);
        Assert.AreEqual(_attachmentDto.CreatedAtUtc, attachment.CreatedAtUtc);

        var modifiedBy = attachment.ModifiedBy;
        Assert.IsNotNull(modifiedBy);
        // ReSharper disable once PossibleNullReferenceException
        Assert.AreEqual(_attachmentDto.ModifiedBy.Guid, modifiedBy.Guid);
        Assert.AreEqual(_attachmentDto.ModifiedBy.FirstName, modifiedBy.FirstName);
        Assert.AreEqual(_attachmentDto.ModifiedBy.LastName, modifiedBy.LastName);
        Assert.AreEqual(_attachmentDto.ModifiedBy.UserName, modifiedBy.UserName);
        Assert.AreEqual(_attachmentDto.ModifiedBy.Email, modifiedBy.Email);
        Assert.AreEqual(_attachmentDto.ModifiedAtUtc, attachment.ModifiedAtUtc);
    }

    [TestMethod]
    public async Task HandlingQuery_Should_CallGetAllForSource_OnAttachmentService()
    {
        // Act
        await _dut.Handle(_query, default);

        // Assert
        _attachmentServiceMock.Verify(u => u.GetAllForSourceAsync(
            _query.FooGuid,
            default), Times.Exactly(1));
    }
}
