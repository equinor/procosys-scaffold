﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.PCS5.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate;

public class Project : PlantEntityBase, IAggregateRoot, ICreationAuditable, IModificationAuditable
{
    public const int NameLengthMax = 30;
    public const int DescriptionLengthMax = 1000;

#pragma warning disable CS8618
    protected Project()
#pragma warning restore CS8618
        : base(null)
    {
    }

    public Project(string plant, Guid proCoSysGuid, string name, string description)
        : base(plant)
    {
        ProCoSysGuid = proCoSysGuid;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Name = name;
    }

    public Guid ProCoSysGuid { get; private set; }
    public string Name { get; private set; }
    public string Description { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAtUtc { get; private set; }
    public int CreatedById { get; private set; }
    public DateTime? ModifiedAtUtc { get; private set; }
    public int? ModifiedById { get; private set; }

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