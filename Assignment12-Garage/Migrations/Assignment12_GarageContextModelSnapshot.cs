﻿// <auto-generated />
using System;
using Assignment12_Garage.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Assignment12_Garage.Migrations
{
    [DbContext(typeof(Assignment12_GarageContext))]
    partial class Assignment12_GarageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Assignment12_Garage.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ArrivalDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NrOfWheels")
                        .HasColumnType("int");

                    b.Property<string>("ParkingSpot")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ParkingSpot");

                    b.Property<string>("RegNumber")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("VehicleModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VehicleType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Vehicle");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ArrivalDate = new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Brand = "Stenaline",
                            Color = "Blue",
                            NrOfWheels = 0,
                            ParkingSpot = "1",
                            RegNumber = "ABC123",
                            VehicleModel = "JokeBoat",
                            VehicleType = 2
                        },
                        new
                        {
                            Id = 2,
                            ArrivalDate = new DateTime(2003, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Brand = "Aston Martin",
                            Color = "Yellow",
                            NrOfWheels = 4,
                            ParkingSpot = "2",
                            RegNumber = "BRUM",
                            VehicleModel = "BRUM",
                            VehicleType = 0
                        },
                        new
                        {
                            Id = 3,
                            ArrivalDate = new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Brand = "Boeing",
                            Color = "White",
                            NrOfWheels = 6,
                            ParkingSpot = "3",
                            RegNumber = "1800FLY",
                            VehicleModel = "OSAIsJustASuggestion",
                            VehicleType = 1
                        },
                        new
                        {
                            Id = 4,
                            ArrivalDate = new DateTime(2013, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Brand = "Volvo",
                            Color = "Deep Blue",
                            NrOfWheels = 6,
                            ParkingSpot = "4",
                            RegNumber = "VTF696",
                            VehicleModel = "Long Boy",
                            VehicleType = 3
                        },
                        new
                        {
                            Id = 5,
                            ArrivalDate = new DateTime(2001, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Brand = "Ford",
                            Color = "Purple",
                            NrOfWheels = 4,
                            ParkingSpot = "5",
                            RegNumber = "424242",
                            VehicleModel = "Fiesta",
                            VehicleType = 0
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
