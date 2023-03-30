﻿using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFoo;
using Equinor.ProCoSys.PCS5.Command.Validators.FooValidators;
using Equinor.ProCoSys.PCS5.Command.Validators.RowVersionValidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.Command.Tests.FooCommands.DeleteFoo;

[TestClass]
public class DeleteFooCommandValidatorTests
{
    private readonly int _fooId = 1;
    private readonly string _rowVersion = "AAAAAAAAABA=";

    private DeleteFooCommandValidator _dut;
    private Mock<IFooValidator> _fooValidatorMock;
    private Mock<IRowVersionValidator> _rowVersionValidatorMock;

    private DeleteFooCommand _command;

    [TestInitialize]
    public void Setup_OkState()
    {
        _fooValidatorMock = new Mock<IFooValidator>();
        _fooValidatorMock.Setup(x => x.FooExistsAsync(_fooId, default)).ReturnsAsync(true);
        _fooValidatorMock.Setup(x => x.FooIsVoidedAsync(_fooId, default)).ReturnsAsync(true);
        _command = new DeleteFooCommand(_fooId, _rowVersion);

        _rowVersionValidatorMock = new Mock<IRowVersionValidator>();
        _rowVersionValidatorMock.Setup(x => x.IsValid(_rowVersion)).Returns(true);

        _dut = new DeleteFooCommandValidator(
            _fooValidatorMock.Object, 
            _rowVersionValidatorMock.Object);
    }

    [TestMethod]
    public async Task Validate_ShouldBeValid_WhenOkState()
    {
        var result = await _dut.ValidateAsync(_command);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public async Task Validate_ShouldFail_When_FooNotVoided()
    {
        _fooValidatorMock.Setup(x => x.FooIsVoidedAsync(_fooId, default)).ReturnsAsync(false);

        var result = await _dut.ValidateAsync(_command);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Foo must be voided before delete!"));
    }

    [TestMethod]
    public async Task Validate_ShouldFail_When_FooNotExists()
    {
        _fooValidatorMock.Setup(inv => inv.FooExistsAsync(_fooId, default))
            .ReturnsAsync(false);

        var result = await _dut.ValidateAsync(_command);

        Assert.IsFalse(result.IsValid);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.IsTrue(result.Errors[0].ErrorMessage.StartsWith("Foo with this ID does not exist!"));
    }
}