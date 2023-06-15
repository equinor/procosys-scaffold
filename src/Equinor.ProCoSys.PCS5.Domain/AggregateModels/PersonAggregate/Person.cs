﻿using System;
using Equinor.ProCoSys.PCS5.Domain.Audit;
using Equinor.ProCoSys.Common.Time;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;

public class Person : EntityBase, IAggregateRoot, IModificationAuditable, IHaveGuid
{
    public const int FirstNameLengthMax = 128;
    public const int LastNameLengthMax = 128;
    public const int UserNameLengthMax = 128;
    public const int EmailLengthMax = 128;

    public Person(Guid guid, string firstName, string lastName, string userName, string email)
    {
        Guid = guid;
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }

    // private setters needed for Entity Framework
    public Guid Guid { get; private set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime? ModifiedAtUtc { get; private set; }
    public int? ModifiedById { get; private set; }

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
