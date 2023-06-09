﻿using Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate;

namespace Equinor.ProCoSys.PCS5.Infrastructure.Repositories;

public class PersonRepository : EntityWithGuidRepository<Person>, IPersonRepository
{
    public PersonRepository(PCS5Context context)
        : base(context, context.Persons) { }
}
