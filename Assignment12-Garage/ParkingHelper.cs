using Assignment12_Garage.Data;
using Assignment12_Garage.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment12_Garage
{
    public class ParkingHelper
    {
        private readonly Assignment12_GarageContext _context;

        public ParkingHelper(Assignment12_GarageContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tries to find a ParkingSpot with other bikes in it 
        /// That also has less than 3 bikes already in it.
        /// Returns empty string if no suitable place was found.
        /// </summary>
        /// <returns>string with positional number</returns>
        public string TryFindMotorcycleSpot()
        {
            string parkingSpot = "";

            var allMotorcycles = _context.Vehicle
                        .Where(v => v.VehicleType == VehicleType.Motorcycle)
                        .GroupBy(v => v.ParkingSpot);

            foreach (var group in allMotorcycles)
            {
                if (group.Count() < 3)
                {
                    parkingSpot = group.Key!;
                    break;
                }
            }
            return parkingSpot;
        }
    }
}
