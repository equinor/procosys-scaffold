﻿using System;
using MediatR;
using ServiceResult;

namespace Equinor.ProCoSys.PCS5.Command.FooCommands.DeleteFoo;

public class DeleteFooCommand : IRequest<Result<Unit>>, IIsFooCommand
{
    public DeleteFooCommand(Guid fooGuid, string rowVersion)
    {
        FooGuid = fooGuid;
        RowVersion = rowVersion;
    }

    public Guid FooGuid { get; }
    public string RowVersion { get; }
}
