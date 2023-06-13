﻿using System;
using System.IO;
using Equinor.ProCoSys.Common;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.PCS5.Domain.Audit;

namespace Equinor.ProCoSys.PCS5.Domain.AggregateModels.AttachmentAggregate;

public class Attachment : EntityBase, IAggregateRoot, ICreationAuditable, IBelongToSource, IHaveGuid
{
    public const int SourceTypeLengthMax = 256;
    public const int FileNameLengthMax = 255;
    public const int BlobPathLengthMax = 1024;

    public Attachment(string sourceType, Guid sourceGuid, string plant, string fileName)
    {
        SourceType = sourceType;
        SourceGuid = sourceGuid;
        FileName = fileName;
        Guid = Guid.NewGuid();
        BlobPath = Path.Combine(plant.Substring(4), SourceType, Guid.ToString()).Replace("\\", "/");
    }

    // private setters needed for Entity Framework
    public string SourceType { get; private set; }
    public Guid SourceGuid { get; private set; }
    public string FileName { get; private set; }
    public string BlobPath { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime? ModifiedAtUtc { get; private set; }
    public int? ModifiedById { get; private set; }
    public Guid Guid { get; private set; }

    public string GetFullBlobPath()
        => Path.Combine(BlobPath, FileName).Replace("\\", "/");

    public void SetCreated(Person createdBy)
    {
        CreatedAtUtc = TimeService.UtcNow;
        if (createdBy == null)
        {
            throw new ArgumentNullException(nameof(createdBy));
        }
        CreatedById = createdBy.Id;
    }

    public void SetModified(Person modifiedBy)
    {
        ModifiedAtUtc = TimeService.UtcNow;
        if (modifiedBy == null)
        {
            throw new ArgumentNullException(nameof(modifiedBy));
        }
        ModifiedById = modifiedBy.Id;
    }
}