﻿// <auto-generated />
using System;
using Equinor.ProCoSys.PCS5.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Equinor.ProCoSys.PCS5.Infrastructure.Migrations
{
    [DbContext(typeof(PCS5Context))]
    [Migration("20230616125623_EnableAuditLog")]
    partial class EnableAuditLog
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.AttachmentAggregate.Attachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("BlobPath")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<int>("RevisionNumber")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<Guid>("SourceGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SourceType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("SourceGuid")
                        .HasDatabaseName("IX_Attachments_SourceGuid");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("SourceGuid"), new[] { "Guid", "FileName", "CreatedById", "CreatedAtUtc", "ModifiedById", "ModifiedAtUtc", "RowVersion" });

                    b.ToTable("Attachments");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("AttachmentsHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.CommentAggregate.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<Guid>("SourceGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SourceType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("SourceGuid")
                        .HasDatabaseName("IX_Comments_SourceGuid");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("SourceGuid"), new[] { "Guid", "Text", "CreatedById", "CreatedAtUtc" });

                    b.ToTable("Comments");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("CommentsHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.FooAggregate.Foo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsVoided")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("Text")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("Guid")
                        .HasDatabaseName("IX_Foos_Guid");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("Guid"), new[] { "Title", "Text", "ProjectId", "CreatedById", "CreatedAtUtc", "ModifiedById", "ModifiedAtUtc", "IsVoided", "RowVersion" });

                    b.HasIndex("ModifiedById");

                    b.HasIndex("ProjectId")
                        .HasDatabaseName("IX_Foos_ProjectId");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("ProjectId"), new[] { "Title", "IsVoided", "RowVersion" });

                    b.ToTable("Foos");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("FoosHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<Guid>("SourceGuid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SourceType")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("SourceGuid")
                        .HasDatabaseName("IX_Links_SourceGuid");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("SourceGuid"), new[] { "Guid", "Url", "Title", "RowVersion" });

                    b.ToTable("Links");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("LinksHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.HasIndex("Guid")
                        .IsUnique();

                    b.HasIndex("ModifiedById");

                    b.ToTable("Persons");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("PersonsHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatedById")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<Guid>("Guid")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeletedInSource")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ModifiedById")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<DateTime>("PeriodEnd")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodEnd");

                    b.Property<DateTime>("PeriodStart")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("PeriodStart");

                    b.Property<string>("Plant")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ModifiedById");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Projects_Name_ASC");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("Name"), new[] { "Plant" });

                    b.HasIndex("Plant")
                        .HasDatabaseName("IX_Projects_Plant_ASC");

                    SqlServerIndexBuilderExtensions.IncludeProperties(b.HasIndex("Plant"), new[] { "Name", "IsClosed", "CreatedAtUtc", "ModifiedAtUtc" });

                    b.ToTable("Projects");

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("ProjectsHistory");
                                ttb
                                    .HasPeriodStart("PeriodStart")
                                    .HasColumnName("PeriodStart");
                                ttb
                                    .HasPeriodEnd("PeriodEnd")
                                    .HasColumnName("PeriodEnd");
                            }));
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.AttachmentAggregate.Attachment", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.CommentAggregate.Comment", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.FooAggregate.Foo", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.LinkAggregate.Link", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);
                });

            modelBuilder.Entity("Equinor.ProCoSys.PCS5.Domain.AggregateModels.ProjectAggregate.Project", b =>
                {
                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Equinor.ProCoSys.PCS5.Domain.AggregateModels.PersonAggregate.Person", null)
                        .WithMany()
                        .HasForeignKey("ModifiedById")
                        .OnDelete(DeleteBehavior.NoAction);
                });
#pragma warning restore 612, 618
        }
    }
}