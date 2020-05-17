﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Trading.Analytics.Domain;

namespace Trading.Analytics.Domain.Migrations
{
    [DbContext(typeof(TradingContext))]
    [Migration("20200517153219_AddDateTime")]
    partial class AddDateTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Trading.Analytics.Domain.PortfolioSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("PriceRub")
                        .HasColumnType("double precision");

                    b.Property<double>("PriceUsd")
                        .HasColumnType("double precision");

                    b.Property<double>("RubPerUsd")
                        .HasColumnType("double precision");

                    b.Property<double>("TotalPriceRub")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("PortfolioSnapshots");
                });

            modelBuilder.Entity("Trading.Analytics.Domain.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Currency")
                        .HasColumnType("text");

                    b.Property<string>("Figi")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Ticker")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("Trading.Analytics.Domain.PositionSnapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("Count")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("PortfolioSnapshotId")
                        .HasColumnType("integer");

                    b.Property<int>("PositionId")
                        .HasColumnType("integer");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioSnapshotId");

                    b.HasIndex("PositionId");

                    b.ToTable("PositionSnapshots");
                });

            modelBuilder.Entity("Trading.Analytics.Domain.PositionSnapshot", b =>
                {
                    b.HasOne("Trading.Analytics.Domain.PortfolioSnapshot", "PortfolioSnapshot")
                        .WithMany("Positions")
                        .HasForeignKey("PortfolioSnapshotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trading.Analytics.Domain.Position", "Position")
                        .WithMany()
                        .HasForeignKey("PositionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
