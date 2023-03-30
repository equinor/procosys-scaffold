﻿using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.Auth.Caches;
using Equinor.ProCoSys.Auth.Person;
using Equinor.ProCoSys.PCS5.Command.PersonCommands.CreatePerson;
using Equinor.ProCoSys.PCS5.Domain;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.PCS5.Test.Common.ExtensionMethods;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PCS5.Command.Tests.PersonCommands.CreatePerson;

[TestClass]
public class CreatePersonCommandHandlerTests : CommandHandlerTestsBase
{
    private Mock<IPersonCache> _personCacheMock;
    private Mock<IPersonRepository> _personRepositoryMock;
    private Mock<IOptionsMonitor<ApplicationOptions>> _optionsMock;
    private readonly string _userName = "VP";
    private readonly string _fistName = "Vippe";
    private readonly string _lastName = "Tangen";
    private readonly string _email = "vp@pcs.com";
    private readonly string _spEmail = "noreply@pcs.com";
    private static readonly string s_azureOid = "8d508aa7-b753-4cb7-b084-cf5508c8ac17";
    private readonly Guid _azureOid = new (s_azureOid);

    private readonly int _personIdOnNew = 1;

    private Person _personAddedToRepository;
    private ProCoSysPerson _proCoSysPerson;

    private CreatePersonCommandHandler _dut;
    private CreatePersonCommand _command;

    [TestInitialize]
    public void Setup()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _personRepositoryMock
            .Setup(x => x.Add(It.IsAny<Person>()))
            .Callback<Person>(person =>
            {
                _personAddedToRepository = person;
                person.SetProtectedIdForTesting(_personIdOnNew);
            });
            
        _proCoSysPerson = new ProCoSysPerson
        {
            UserName = _userName,
            FirstName = _fistName,
            LastName = _lastName,
            Email = _email,
            AzureOid = s_azureOid,
            ServicePrincipal = false
        };
        _personCacheMock = new Mock<IPersonCache>();
        _personCacheMock
            .Setup(x => x.GetAsync(_azureOid))
            .ReturnsAsync(_proCoSysPerson);

        _optionsMock = new Mock<IOptionsMonitor<ApplicationOptions>>();
        _optionsMock.Setup(o => o.CurrentValue).Returns(
            new ApplicationOptions
            {
                ServicePrincipalMail = _spEmail
            });

        _command = new CreatePersonCommand(_azureOid);

        _dut = new CreatePersonCommandHandler(
            _personCacheMock.Object,
            _personRepositoryMock.Object,
            UnitOfWorkMock.Object,
            _optionsMock.Object);
    }

    [TestMethod]
    public async Task HandlingCommand_ShouldAddPersonToRepository_WhenPersonNotExists()
    {
        // Act
        await _dut.Handle(_command, default);

        // Assert
        Assert.IsNotNull(_personAddedToRepository);
        Assert.AreEqual(_personIdOnNew, _personAddedToRepository.Id);
        Assert.AreEqual(_userName, _personAddedToRepository.UserName);
        Assert.AreEqual(_fistName, _personAddedToRepository.FirstName);
        Assert.AreEqual(_lastName, _personAddedToRepository.LastName);
        Assert.AreEqual(_email, _personAddedToRepository.Email);
    }

    [TestMethod]
    public async Task HandlingCommand_ShouldAddServicePrincipalToRepository_WhenPersonNotExists()
    {
        // Arrange
        _proCoSysPerson.Email = null;
        _proCoSysPerson.ServicePrincipal = true;

        // Act
        await _dut.Handle(_command, default);

        // Assert
        Assert.IsNotNull(_personAddedToRepository);
        Assert.AreEqual(_personIdOnNew, _personAddedToRepository.Id);
        Assert.AreEqual(_userName, _personAddedToRepository.UserName);
        Assert.AreEqual(_fistName, _personAddedToRepository.FirstName);
        Assert.AreEqual(_lastName, _personAddedToRepository.LastName);
        Assert.AreEqual(_spEmail, _personAddedToRepository.Email);
    }

    [TestMethod]
    public async Task HandlingCommand_ShouldNotAddPersonToRepository_WhenPersonAlreadyExists()
    {
        // Arrange
        _personRepositoryMock.Setup(p => p.GetByOidAsync(_azureOid))
            .ReturnsAsync(new Person(_azureOid, _fistName, _lastName, _userName, _email));

        // Act
        await _dut.Handle(_command, default);

        // Assert
        Assert.IsNull(_personAddedToRepository);
    }

    [TestMethod]
    public async Task HandlingCommand_ShouldSave()
    {
        // Act
        await _dut.Handle(_command, default);

        // Assert
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [TestMethod]
    public async Task HandlingCommand_ShouldThrewException_WhenPersonNotInCache()
    {
        // Arrange
        _personCacheMock.Setup(x => x.GetAsync(_azureOid)).ReturnsAsync((ProCoSysPerson)null);

        // Act and Assert
        await Assert.ThrowsExceptionAsync<Exception>(() => _dut.Handle(_command, default));
    }
}