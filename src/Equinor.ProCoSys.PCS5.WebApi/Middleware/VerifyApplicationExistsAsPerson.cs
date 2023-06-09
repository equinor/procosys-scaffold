﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.PCS5.Command.PersonCommands.CreatePerson;
using Equinor.ProCoSys.PCS5.WebApi.Authentication;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.PCS5.WebApi.Middleware;

/// <summary>
/// Ensure that PCS5ApiObjectId (i.e the application) exists as Person.
/// Needed when application modifies data, setting ModifiedById for changed records
/// </summary>
public class VerifyApplicationExistsAsPerson : IHostedService
{
    private readonly IServiceScopeFactory _serviceProvider;
    private readonly IOptionsMonitor<PCS5AuthenticatorOptions> _options;
    private readonly ILogger<VerifyApplicationExistsAsPerson> _logger;

    public VerifyApplicationExistsAsPerson(
        IServiceScopeFactory serviceProvider,
        IOptionsMonitor<PCS5AuthenticatorOptions> options, 
        ILogger<VerifyApplicationExistsAsPerson> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var mediator =
            scope.ServiceProvider
                .GetRequiredService<IMediator>();
        var currentUserSetter =
            scope.ServiceProvider
                .GetRequiredService<ICurrentUserSetter>();

        var oid = _options.CurrentValue.PCS5ApiObjectId;
        _logger.LogInformation($"Ensuring '{oid}' exists as Person");
        try
        {
            currentUserSetter.SetCurrentUserOid(oid);
            await mediator.Send(new CreatePersonCommand(oid), cancellationToken);
            _logger.LogInformation($"'{oid}' ensured");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Exception handling {nameof(CreatePersonCommand)}");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}