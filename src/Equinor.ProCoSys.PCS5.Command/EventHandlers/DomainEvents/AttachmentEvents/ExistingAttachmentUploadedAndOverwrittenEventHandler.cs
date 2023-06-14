﻿using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PCS5.Domain.Events.DomainEvents.AttachmentEvents;
using MediatR;

namespace Equinor.ProCoSys.PCS5.Command.EventHandlers.DomainEvents.AttachmentEvents;

public class ExistingAttachmentUploadedAndOverwrittenEventHandler : INotificationHandler<ExistingAttachmentUploadedAndOverwrittenEvent>
{
    // todo unit test
    public Task Handle(ExistingAttachmentUploadedAndOverwrittenEvent notification, CancellationToken cancellationToken)
    {
        var sourceGuid = notification.Attachment.SourceGuid;

        // ToDo Send event to the bus
        return Task.CompletedTask;
    }
}
