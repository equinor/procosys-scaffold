﻿using Equinor.ProCoSys.PCS5.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Controllers;

[TestClass]
public class OverwriteAttachmentDtoValidatorTests : UploadBaseDtoValidatorTests<OverwriteAttachmentDto>
{
    private readonly string _rowVersion = "AAAAAAAAABA=";
    private Mock<IRowVersionValidator> _rowVersionValidatorMock;
    
    protected override void SetupDut()
    {
        _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
        _rowVersionValidatorMock.Setup(x => x.IsValid(_rowVersion))
            .Returns(true);
        _dut = new OverwriteAttachmentDtoValidator(
            _rowVersionValidatorMock.Object,
            _blobStorageOptionsMock.Object);
    }

    protected override OverwriteAttachmentDto GetValidDto() =>
        new()
        {
            File = new TestableFormFile("picture.gif", 1000),
            RowVersion = _rowVersion
        };
}
