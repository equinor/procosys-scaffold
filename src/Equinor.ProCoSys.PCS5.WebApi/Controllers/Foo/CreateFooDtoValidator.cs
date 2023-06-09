﻿using FluentValidation;

namespace Equinor.ProCoSys.PCS5.WebApi.Controllers.Foo
{
    public class CreateFooDtoValidator : AbstractValidator<CreateFooDto>
    {
        public CreateFooDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto).NotNull();

            RuleFor(dto => dto.Title)
                .NotNull()
                .MinimumLength(Domain.AggregateModels.FooAggregate.Foo.TitleLengthMin)
                .MaximumLength(Domain.AggregateModels.FooAggregate.Foo.TitleLengthMax);

            RuleFor(dto => dto.ProjectName)
                .NotNull();
        }
    }
}
