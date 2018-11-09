﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Naos.Sample.UserAccounts.EntityFramework;

namespace Naos.Sample.UserAccounts.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(UserAccountContext))]
    partial class UserAccountContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Naos.Sample.UserAccounts.Domain.UserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<string>("TenantId");

                    b.Property<string>("Test");

                    b.Property<int>("VisitCount");

                    b.HasKey("Id");

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("Naos.Sample.UserAccounts.Domain.UserAccount", b =>
                {
                    b.OwnsOne("Naos.Core.Common.DateTimeEpoch", "LastVisitDate", b1 =>
                        {
                            b1.Property<Guid>("UserAccountId");

                            b1.Property<DateTime>("DateTime");

                            b1.ToTable("UserAccounts");

                            b1.HasOne("Naos.Sample.UserAccounts.Domain.UserAccount")
                                .WithOne("LastVisitDate")
                                .HasForeignKey("Naos.Core.Common.DateTimeEpoch", "UserAccountId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });

                    b.OwnsOne("Naos.Core.Common.DateTimeEpoch", "RegisterDate", b1 =>
                        {
                            b1.Property<Guid>("UserAccountId");

                            b1.Property<DateTime>("DateTime");

                            b1.ToTable("UserAccounts");

                            b1.HasOne("Naos.Sample.UserAccounts.Domain.UserAccount")
                                .WithOne("RegisterDate")
                                .HasForeignKey("Naos.Core.Common.DateTimeEpoch", "UserAccountId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
