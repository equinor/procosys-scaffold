﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;

namespace Equinor.ProCoSys.PCS5.Domain.Audit;

/// <summary>
/// Interface to get and set creation data on an entity.
/// The methods are used by the context and should NOT be used by anyone else.
/// </summary>
public interface ICreationAuditable
{
    DateTime CreatedAtUtc { get; }
    int CreatedById { get; }

    /// <summary>
    /// Method to set creation data on an entity.
    /// This is used by the context and should NOT be used by anyone else.
    /// </summary>
    /// <param name="createdBy">The user who created the entity</param>
    void SetCreated(Person createdBy);
}