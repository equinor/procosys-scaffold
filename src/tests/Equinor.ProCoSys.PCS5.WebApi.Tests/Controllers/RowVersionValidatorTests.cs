﻿using Equinor.ProCoSys.PCS5.WebApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PCS5.WebApi.Tests.Controllers;

[TestClass]
public class RowVersionValidatorTests
{
    private RowVersionValidator _dut;

    [TestInitialize]
    public void SetUp() => _dut = new RowVersionValidator();

    [TestMethod]
    public void IsValid_ValidRowVersion_ShouldReturnTrue()
    { 
        var validRowVersion = "AAAAAAAAABA=";

        var result = _dut.IsValid(validRowVersion);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValid_InvalidRowVersion_ShouldReturnFalse()
    {
        var invalidRowVersion = "String";

        var result = _dut.IsValid(invalidRowVersion);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_EmptyString_ShouldReturnFalse()
    {
        var result = _dut.IsValid(string.Empty);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_Null_ShouldReturnFalse()
    {
        var result = _dut.IsValid(null!);
        Assert.IsFalse(result);
    }
}
