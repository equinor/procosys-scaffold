﻿using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Controllers;

[TestClass]
public class CreateCommentDtoValidatorTests
{
    private readonly CreateCommentDtoValidator _dut = new();

    [TestMethod]
    public async Task Validate_ShouldBeValid_WhenOkState()
    {
        // Arrange
        var dto = new CreateCommentDto { Text = "New text" };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTextNotGiven()
    {
        // Arrange
        var dto = new CreateCommentDto();

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("'Text' must not be empty."));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_WhenTextIsTooLongAsync()
    {
        // Arrange
        var dto = new CreateCommentDto
        {
            Text = new string('x', Domain.AggregateModels.CommentAggregate.Comment.TextLengthMax + 1),
            
        };

        // Act
        var result = await _dut.ValidateAsync(dto);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("The length of 'Text' must be"));
    }
}
