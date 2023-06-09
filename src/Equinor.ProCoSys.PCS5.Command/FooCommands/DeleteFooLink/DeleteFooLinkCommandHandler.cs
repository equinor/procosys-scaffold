﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Command.Links;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFooLink;

public class DeleteFooLinkCommandHandler : IRequestHandler<DeleteFooLinkCommand, Result<Unit>>
{
    private readonly ILinkService _linkService;

    public DeleteFooLinkCommandHandler(ILinkService linkService) => _linkService = linkService;

    public async Task<Result<Unit>> Handle(DeleteFooLinkCommand request, CancellationToken cancellationToken)
    {
        await _linkService.DeleteAsync(
            request.LinkGuid,
            request.RowVersion,
            cancellationToken);

        return new SuccessResult<Unit>(Unit.Value);
    }
}
