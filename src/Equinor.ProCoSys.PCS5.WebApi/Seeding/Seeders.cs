﻿using System;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;

namespace Equinor.ProCoSys.PCS5.WebApi.Seeding;

public static class Seeders
{
    public static void AddUsers(this IPersonRepository personRepository, int entryCount)
    {
        for (var i = 0; i < entryCount; i++)
        {
            var user = new Person(Guid.NewGuid(), $"Firstname-{i}", $"Lastname-{i}", $"username-{i}", $"username-{i}@pcs.pcs");
            personRepository.Add(user);
        }
    }
}