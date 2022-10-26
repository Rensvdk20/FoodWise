﻿// <auto-generated />
using System;
using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(FoodWiseDbContext))]
    [Migration("20221026154248_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domain.Canteen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("City")
                        .HasColumnType("int");

                    b.Property<int>("Location")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Canteens");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            City = 0,
                            Location = 1
                        },
                        new
                        {
                            Id = 2,
                            City = 0,
                            Location = 0
                        });
                });

            modelBuilder.Entity("Domain.CanteenEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CanteenId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EmployeeNumber")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CanteenId");

                    b.ToTable("CanteenEmployees");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CanteenId = 1,
                            Email = "Helma@avanscanteen.nl",
                            EmployeeNumber = 555,
                            Name = "Helma"
                        },
                        new
                        {
                            Id = 2,
                            CanteenId = 2,
                            Email = "Erika@avanscanteen.nl",
                            EmployeeNumber = 678,
                            Name = "Erika"
                        });
                });

            modelBuilder.Entity("Domain.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("AvailableTill")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CanteenId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("EighteenPlus")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<DateTime>("PickupTime")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Price")
                        .HasColumnType("smallmoney");

                    b.Property<int?>("ReservedByStudentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CanteenId");

                    b.HasIndex("ReservedByStudentId");

                    b.ToTable("Packages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AvailableTill = new DateTime(2022, 11, 16, 17, 15, 0, 0, DateTimeKind.Unspecified),
                            CanteenId = 1,
                            Category = 1,
                            Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                            EighteenPlus = false,
                            Name = "Gezond pakket",
                            PickupTime = new DateTime(2022, 11, 16, 13, 15, 0, 0, DateTimeKind.Unspecified),
                            Price = 4.50m
                        },
                        new
                        {
                            Id = 2,
                            AvailableTill = new DateTime(2022, 11, 18, 16, 15, 0, 0, DateTimeKind.Unspecified),
                            CanteenId = 2,
                            Category = 1,
                            Description = "Heerlijke broodjes als lunch of als tussendoortje",
                            EighteenPlus = true,
                            Name = "Broodjes pakket",
                            PickupTime = new DateTime(2022, 11, 18, 14, 15, 0, 0, DateTimeKind.Unspecified),
                            Price = 6.50m
                        },
                        new
                        {
                            Id = 3,
                            AvailableTill = new DateTime(2022, 11, 16, 17, 15, 0, 0, DateTimeKind.Unspecified),
                            CanteenId = 1,
                            Category = 2,
                            Description = "Verschillende soorten soep in een compact pakket",
                            EighteenPlus = false,
                            Name = "Soep pakket",
                            PickupTime = new DateTime(2022, 11, 16, 15, 15, 0, 0, DateTimeKind.Unspecified),
                            Price = 7.50m,
                            ReservedByStudentId = 1
                        });
                });

            modelBuilder.Entity("Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Picture")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("containsAlcohol")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Appel",
                            containsAlcohol = false
                        },
                        new
                        {
                            Id = 2,
                            Name = "Peer",
                            containsAlcohol = false
                        },
                        new
                        {
                            Id = 3,
                            Name = "Broodje Frikandel",
                            containsAlcohol = false
                        },
                        new
                        {
                            Id = 4,
                            Name = "Kaiserbroodje",
                            containsAlcohol = false
                        },
                        new
                        {
                            Id = 5,
                            Name = "Hertog Jan",
                            containsAlcohol = true
                        });
                });

            modelBuilder.Entity("Domain.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(254)
                        .HasColumnType("nvarchar(254)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("StudentNumber")
                        .HasMaxLength(12)
                        .HasColumnType("int");

                    b.Property<int>("StudyCity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Students");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Birthday = new DateTime(2001, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "Mark@gmail.com",
                            FirstName = "Mark",
                            LastName = "De Groot",
                            PhoneNumber = "+31612345678",
                            StudentNumber = 2184500,
                            StudyCity = 0
                        });
                });

            modelBuilder.Entity("Package_Product", b =>
                {
                    b.Property<int>("PackageId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.HasKey("PackageId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Package_Product");

                    b.HasData(
                        new
                        {
                            PackageId = 1,
                            ProductId = 1
                        },
                        new
                        {
                            PackageId = 1,
                            ProductId = 2
                        },
                        new
                        {
                            PackageId = 2,
                            ProductId = 3
                        },
                        new
                        {
                            PackageId = 2,
                            ProductId = 4
                        },
                        new
                        {
                            PackageId = 2,
                            ProductId = 5
                        });
                });

            modelBuilder.Entity("Domain.CanteenEmployee", b =>
                {
                    b.HasOne("Domain.Canteen", "Canteen")
                        .WithMany()
                        .HasForeignKey("CanteenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Canteen");
                });

            modelBuilder.Entity("Domain.Package", b =>
                {
                    b.HasOne("Domain.Canteen", "Canteen")
                        .WithMany("Packages")
                        .HasForeignKey("CanteenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Student", "ReservedByStudent")
                        .WithMany()
                        .HasForeignKey("ReservedByStudentId");

                    b.Navigation("Canteen");

                    b.Navigation("ReservedByStudent");
                });

            modelBuilder.Entity("Package_Product", b =>
                {
                    b.HasOne("Domain.Package", null)
                        .WithMany()
                        .HasForeignKey("PackageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Canteen", b =>
                {
                    b.Navigation("Packages");
                });
#pragma warning restore 612, 618
        }
    }
}