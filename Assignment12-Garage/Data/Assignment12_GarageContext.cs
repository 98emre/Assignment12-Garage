using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Assignment12_Garage.Models;

namespace Assignment12_Garage.Data
{
    public class Assignment12_GarageContext : DbContext
    {
        public Assignment12_GarageContext(DbContextOptions<Assignment12_GarageContext> options)
            : base(options)
        {
        }

        public DbSet<Assignment12_Garage.Models.Vehicle> Vehicle { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle { Id = 1, VehicleType = VehicleType.Boat, RegNumber = "ABC123", Color = "Blue", Brand = "Stenaline", VehicleModel = "JokeBoat", NrOfWheels = 0, ArrivalDate = DateTime.Parse("2023-01-05") },
                new Vehicle { Id = 2, VehicleType = VehicleType.Car, RegNumber = "BRUM", Color = "Yellow", Brand = "Aston Martin", VehicleModel = "BRUM", NrOfWheels = 4, ArrivalDate = DateTime.Parse("2003-01-05") },
                new Vehicle { Id = 3, VehicleType = VehicleType.Airplane, RegNumber = "1800FLY", Color = "White", Brand = "Boeing", VehicleModel = "OSAIsJustASuggestion", NrOfWheels = 6, ArrivalDate = DateTime.Parse("2023-01-05") },
                new Vehicle { Id = 4, VehicleType = VehicleType.Bus, RegNumber = "VTF696", Color = "Deep Blue", Brand = "Volvo", VehicleModel = "Long Boy", NrOfWheels = 6, ArrivalDate = DateTime.Parse("2013-01-05") },
                new Vehicle { Id = 5, VehicleType = VehicleType.Car, RegNumber = "424242", Color = "Purple", Brand = "Ford", VehicleModel = "Fiesta", NrOfWheels = 4, ArrivalDate = DateTime.Parse("2001-01-05") }
                );
        }
    }
}
