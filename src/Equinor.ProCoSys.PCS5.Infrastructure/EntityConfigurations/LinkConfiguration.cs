﻿using Equinor.ProCoSys.PCS5.Domain.AggregateModels.CommentAggregate;
using Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate;
using Equinor.ProCoSys.PCS5.Infrastructure.EntityConfigurations.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.PCS5.Infrastructure.EntityConfigurations;

internal class LinkConfiguration : IEntityTypeConfiguration<Link>
{
    public void Configure(EntityTypeBuilder<Link> builder)
    {
        builder.ConfigureSystemVersioning();
        builder.ConfigureCreationAudit();
        builder.ConfigureModificationAudit();
        builder.ConfigureConcurrencyToken();

        builder.ToTable(t => t.IsTemporal());

        builder.Property(x => x.SourceType)
            .HasMaxLength(Comment.SourceTypeLengthMax)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(Link.TitleLengthMax)
            .IsRequired();

        builder.Property(x => x.Url)
            .HasMaxLength(Link.UrlLengthMax)
            .IsRequired();

        builder
            .HasIndex(x => x.SourceGuid)
            .HasDatabaseName("IX_Links_SourceGuid")
            .IncludeProperties(x => new
            {
                x.Guid,
                x.Url,
                x.Title,
                x.RowVersion
            });
    }
}
