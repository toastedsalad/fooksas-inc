﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TableMgmtApp.Persistence;

#nullable disable

namespace TableMgmtApp.Migrations
{
    [DbContext(typeof(TableMgmtAppDbContext))]
    partial class TableMgmtAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("TableMgmtApp.Discount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Rate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Discounts", (string)null);
                });

            modelBuilder.Entity("TableMgmtApp.PlaySession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DiscountId")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("PlayTime")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PlayerId")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TableNumber")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlaySessions", (string)null);
                });

            modelBuilder.Entity("TableMgmtApp.Player", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DiscountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DiscountId");

                    b.ToTable("Players", (string)null);
                });

            modelBuilder.Entity("TableMgmtApp.PoolTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("PoolTables", (string)null);
                });

            modelBuilder.Entity("TableMgmtApp.ScheduleDTO", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DefaultRate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("WeeklyRates")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Schedules", (string)null);
                });

            modelBuilder.Entity("TableMgmtApp.PlaySession", b =>
                {
                    b.HasOne("TableMgmtApp.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("TableMgmtApp.Player", "Player")
                        .WithMany("PlaySessions")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Discount");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("TableMgmtApp.Player", b =>
                {
                    b.HasOne("TableMgmtApp.Discount", "Discount")
                        .WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Discount");
                });

            modelBuilder.Entity("TableMgmtApp.Player", b =>
                {
                    b.Navigation("PlaySessions");
                });
#pragma warning restore 612, 618
        }
    }
}
