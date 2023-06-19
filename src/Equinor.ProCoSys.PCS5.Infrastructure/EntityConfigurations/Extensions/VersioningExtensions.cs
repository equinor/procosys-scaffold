﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Equinor.ProCoSys.PCS5.Infrastructure.EntityConfigurations.Extensions;

public static class VersioningExtensions
{
    public static void ConfigureSystemVersioning(this EntityTypeBuilder builder) =>
        builder.ToTable(t => t.IsTemporal());
}
