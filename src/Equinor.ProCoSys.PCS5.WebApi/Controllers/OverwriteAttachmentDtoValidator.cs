﻿using FluentValidation;
using Equinor.ProCoSys.BlobStorage;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.PCS5.WebApi.Controllers;

public class OverwriteAttachmentDtoValidator : UploadBaseDtoValidator<OverwriteAttachmentDto>
{
    public OverwriteAttachmentDtoValidator(IRowVersionValidator rowVersionValidator,
        IOptionsSnapshot<BlobStorageOptions> blobStorageOptions)
        : base(blobStorageOptions)
    {
        RuleFor(dto => dto.RowVersion)
            .NotNull()
            .Must(HaveValidRowVersion)
            .WithMessage(dto => $"Dto does not have valid rowVersion! RowVersion={dto.RowVersion}");

        bool HaveValidRowVersion(string rowVersion)
            => rowVersionValidator.IsValid(rowVersion);
    }
}
