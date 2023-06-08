﻿using FluentValidation;

namespace Equinor.ProCoSys.PCS5.WebApi.Controllers
{
    public class CreateLinkDtoValidator : AbstractValidator<CreateLinkDto>
    {
        public CreateLinkDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(dto => dto).NotNull();

            RuleFor(dto => dto.Title)
                .NotNull()
                .MaximumLength(Domain.AggregateModels.LinkAggregate.Link.TitleLengthMax);

            RuleFor(dto => dto.Url)
                .NotNull()
                .MaximumLength(Domain.AggregateModels.LinkAggregate.Link.UrlLengthMax);
        }
    }
}
