﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BlobStorage;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.PCS5.Domain;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.AttachmentAggregate;
using Equinor.ProCoSys.PCS5.Domain.Events.DomainEvents.AttachmentEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.PCS5.Command.Attachments;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IPlantProvider _plantProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAzureBlobService _azureBlobService;
    private readonly IOptionsSnapshot<BlobStorageOptions> _blobStorageOptions;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        IAttachmentRepository attachmentRepository,
        IPlantProvider plantProvider,
        IUnitOfWork unitOfWork,
        IAzureBlobService azureBlobService,
        IOptionsSnapshot<BlobStorageOptions> blobStorageOptions,
        ILogger<AttachmentService> logger)
    {
        _attachmentRepository = attachmentRepository;
        _plantProvider = plantProvider;
        _unitOfWork = unitOfWork;
        _azureBlobService = azureBlobService;
        _blobStorageOptions = blobStorageOptions;
        _logger = logger;
    }

    public async Task<AttachmentDto> UploadNewAsync(
        string sourceType,
        Guid sourceGuid,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.TryGetAttachmentWithFilenameForSourceAsync(sourceGuid, fileName);

        if (attachment != null)
        {
            throw new Exception($"{sourceType} {sourceGuid} already has an attachment with filename {fileName}");
        }

        attachment = new Attachment(
            sourceType,
            sourceGuid,
            _plantProvider.Plant,
            fileName);
        _attachmentRepository.Add(attachment);
        attachment.AddDomainEvent(new NewAttachmentUploadedEvent(attachment));

        return await UploadAsync(attachment, content, false, cancellationToken);
    }

    public async Task<AttachmentDto> UploadOverwriteAsync(
        string sourceType,
        Guid sourceGuid,
        string fileName,
        Stream content,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.TryGetAttachmentWithFilenameForSourceAsync(sourceGuid, fileName);

        if (attachment == null)
        {
            throw new Exception($"{sourceType} {sourceGuid} don't have an attachment with filename {fileName}");
        }

        attachment.IncreaseRevisionNumber();

        attachment.SetRowVersion(rowVersion);
        attachment.AddDomainEvent(new ExistingAttachmentUploadedAndOverwrittenEvent(attachment));

        return await UploadAsync(attachment, content, true, cancellationToken);
    }

    public async Task<bool> FilenameExistsForSourceAsync(Guid sourceGuid, string fileName)
    {
        var attachment = await _attachmentRepository.TryGetAttachmentWithFilenameForSourceAsync(sourceGuid, fileName);
        return attachment != null;
    }

    public async Task DeleteAsync(
        Guid guid,
        string rowVersion,
        CancellationToken cancellationToken)
    {
        var attachment = await _attachmentRepository.TryGetByGuidAsync(guid);

        if (attachment == null)
        {
            throw new Exception($"Attachment with guid {guid} not found when updating");
        }

        // Setting RowVersion before delete has 2 missions:
        // 1) Set correct Concurrency
        // 2) Trigger the update of modifiedBy / modifiedAt to be able to log who performed the deletion
        attachment.SetRowVersion(rowVersion);
        _attachmentRepository.Remove(attachment);
        attachment.AddDomainEvent(new AttachmentDeletedEvent(attachment));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug($"Attachment '{attachment.FileName}' with guid {attachment.Guid} deleted for {attachment.SourceGuid}");
    }

    private async Task<AttachmentDto> UploadAsync(
        Attachment attachment,
        Stream content,
        bool overwriteIfExists,
        CancellationToken cancellationToken)
    {
        var fullBlobPath = attachment.GetFullBlobPath();
        await _azureBlobService.UploadAsync(
            _blobStorageOptions.Value.BlobContainer,
            fullBlobPath,
            content,
            overwriteIfExists,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogDebug($"Attachment '{attachment.FileName}' with guid {attachment.Guid} uploaded for {attachment.SourceGuid}");

        return new AttachmentDto(attachment.Guid, attachment.RowVersion.ConvertToString());
    }

    public async Task<bool> ExistsAsync(Guid guid)
    {
        var attachment = await _attachmentRepository.TryGetByGuidAsync(guid);
        return attachment != null;
    }
}
