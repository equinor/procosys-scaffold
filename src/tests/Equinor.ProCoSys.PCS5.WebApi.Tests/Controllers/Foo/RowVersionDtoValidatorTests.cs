﻿using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.WebApi.Controllers;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Controllers.Foo;

[TestClass]
public class RowVersionDtoValidatorTests
{
    private readonly string _rowVersion = "AAAAAAAAABA=";

    private RowVersionDtoValidator _dut;
    private Mock<IRowVersionValidator> _rowVersionValidatorMock;

    [TestInitialize]
    public void Setup_OkState()
    {
        _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
        _rowVersionValidatorMock.Setup(x => x.IsValid(_rowVersion)).Returns(true);

        _dut = new RowVersionDtoValidator(_rowVersionValidatorMock.Object);
    }

    [TestMethod]
    public async Task Validate_ShouldBeValid_WhenOkState()
    {
        // Arrange
        var dto = new RowVersionDto { RowVersion = _rowVersion };
        
        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenRowVersionNotGiven()
    {
        // Arrange
        var dto = new RowVersionDto();

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Row Version' must not be empty."));
    }
}
