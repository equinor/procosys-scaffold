﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.Common.Misc;
using MediatR;
using Microsoft.Extensions.Logging;
using ServiceResult;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.FooAggregate;
using Equinor.ProCoSys.PCS5.Domain;
using Equinor.ProCoSys.PCS5.Domain.Events.DomainEvents.FooEvents;

namespace Equinor.ProCoSys.PCS5.Command.FooCommands.EditFoo;

public class EditFooCommandHandler : IRequestHandler<EditFooCommand, Result<string>>
{
    private readonly IFooRepository _fooRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EditFooCommandHandler> _logger;

    public EditFooCommandHandler(
        IFooRepository fooRepository,
        IUnitOfWork unitOfWork,
        ILogger<EditFooCommandHandler> logger)
    {
        _fooRepository = fooRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(EditFooCommand request, CancellationToken cancellationToken)
    {
        var foo = await _fooRepository.GetByGuidAsync(request.FooGuid);
        if (foo == null)
        {
            throw new Exception($"Entity {nameof(Foo)} {request.FooGuid} not found");
        }

        foo.EditFoo(request.Title, request.Text);
        foo.SetRowVersion(request.RowVersion);
        foo.AddDomainEvent(new FooUpdatedEvent(foo));

        await _unitOfWork.SaveChangesAsync(cancellationToken);
            
        _logger.LogInformation($"Foo '{request.Title}' updated");
            
        return new SuccessResult<string>(foo.RowVersion.ConvertToString());
    }
}
