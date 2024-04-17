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
        public Assignment12_GarageContext (DbContextOptions<Assignment12_GarageContext> options)
            : base(options)
        {
        }

        public DbSet<Assignment12_Garage.Models.Vehicle> Vehicle { get; set; } = default!;
    }
}
