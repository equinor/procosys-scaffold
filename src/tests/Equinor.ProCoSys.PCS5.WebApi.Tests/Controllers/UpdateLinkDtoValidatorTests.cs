﻿using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Controllers;

[TestClass]
public class UpdateLinkDtoValidatorTests
{
    private readonly string _rowVersion = "AAAAAAAAABA=";

    private UpdateLinkDtoValidator _dut;
    private Mock<IRowVersionValidator> _rowVersionValidatorMock;

    [TestInitialize]
    public void Setup_OkState()
    {
        _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
        _rowVersionValidatorMock.Setup(x => x.IsValid(_rowVersion)).Returns(true);

        _dut = new UpdateLinkDtoValidator(_rowVersionValidatorMock.Object);
    }

    [TestMethod]
    public async Task Validate_ShouldBeValid_WhenOkState()
    {
        // Arrange
        var dto = new UpdateLinkDto { Title = "New title", Url = "U", RowVersion = _rowVersion };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTitleNotGiven()
    {
        // Arrange
        var dto = new UpdateLinkDto { Url = "U", RowVersion = _rowVersion };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Title' must not be empty."));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTitleIsTooLongAsync()
    {
        // Arrange
        var dto = new UpdateLinkDto
        {
            Title = new string('x', Domain.AggregateModels.LinkAggregate.Link.TitleLengthMax + 1),
            Url = "U",
            RowVersion = _rowVersion
        };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("The length of 'Title' must be"));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenUrlNotGiven()
    {
        // Arrange
        var dto = new UpdateLinkDto { Title = "New title", RowVersion = _rowVersion };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Url' must not be empty."));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenUrlIsTooLongAsync()
    {
        // Arrange
        var dto = new UpdateLinkDto
        {
            Title = "New title",
            Url = new string('x', Domain.AggregateModels.LinkAggregate.Link.UrlLengthMax + 1),
            RowVersion = _rowVersion
        };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("The length of 'Url' must be"));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenRowVersionNotGiven()
    {
        // Arrange
        var dto = new UpdateLinkDto { Title = "New title" };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Row Version' must not be empty."));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenIllegalRowVersion()
    {
        // Arrange
        _rowVersionValidatorMock.Setup(x => x.IsValid(_rowVersion)).Returns(false);
        var dto = new UpdateLinkDto { Title = "New title", RowVersion = _rowVersion };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Dto does not have valid rowVersion!"));
    }
}
