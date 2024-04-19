using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment12_Garage.Data;
using Assignment12_Garage.Models;
using Assignment12_Garage.Models.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Assignment12_Garage.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Assignment12_GarageContext _context;
        private const int MaxParkingSpaces = 25;
        private List<string> ParkingSpots;

        public VehiclesController(Assignment12_GarageContext context)
        {
            _context = context;

            ParkingSpots = new List<string>();
            var parkedVehicles = _context.Vehicle.Select(v => v.ParkingSpot).ToList();

            for (int i = 1; i <= MaxParkingSpaces; i++)
            {
                if(parkedVehicles.Contains(i.ToString()))
                {
                    ParkingSpots.Add(i.ToString());
                }

                else
                {
                    ParkingSpots.Add(null);
                }
            }
        }

        private string FindParkingSpot()
        {
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                if (ParkingSpots[i] == null)
                {
                    int parkingSpotNumber = i + 1;
                    ParkingSpots[i] = parkingSpotNumber.ToString();
                    return ParkingSpots[i];
                }
            }

            return null;
        }

        [HttpGet]
        public IActionResult Statistics()
        {
            var parkedVehicles = _context.Vehicle.ToList();

            var vehicleTypeCount = new Dictionary<VehicleType, int>();

            foreach (var vehicle in parkedVehicles)
            {
                if (!vehicleTypeCount.ContainsKey(vehicle.VehicleType))
                {
                    vehicleTypeCount[vehicle.VehicleType] = 0;
                }

                vehicleTypeCount[vehicle.VehicleType]++;
            }


            var totalWheels = parkedVehicles.Sum(v => v.NrOfWheels);

            ViewBag.VehicleType = vehicleTypeCount;
            ViewBag.TotalWheels = totalWheels;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            int availableSpaces = MaxParkingSpaces - _context.Vehicle.Count();
            ViewBag.AvailableSpaces = availableSpaces;

            var vehicles = _context.Vehicle.AsQueryable();

            switch (sortOrder)
            {
                case "vehicleType":
                    vehicles = vehicles.OrderBy(v => v.VehicleType);
                    break;
                case "regNumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber);
                    break;
                case "arrivalDate":
                    vehicles = vehicles.OrderBy(v => v.ArrivalDate);
                    break;
                case "parkingSpot":
                    vehicles = vehicles.OrderBy(v => v.ParkingSpot);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Id);
                    break;
            }

            var sortedVehicles = await vehicles
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType,
                            RegNumber = v.RegNumber,
                            ArrivalDate = v.ArrivalDate,
                            ParkingSpot = v.ParkingSpot
                        }).ToListAsync();

            return View("Index", sortedVehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string regNumber, string color, string brand)
        {
            int availableSpaces = MaxParkingSpaces - _context.Vehicle.Count();
            ViewBag.AvailableSpaces = availableSpaces;

            if (string.IsNullOrEmpty(regNumber) && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(brand))
            {
                TempData["SearchFail"] = "Please provide input for at least one search criteria.";
                return RedirectToAction("Index");
            }

            var query = _context.Vehicle.AsQueryable();

            if (!string.IsNullOrEmpty(regNumber))
            {
                query = query.Where(v => v.RegNumber.Equals(regNumber.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(v => v.Color.Equals(color.Trim()));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v => v.Brand.Equals(brand.Trim()));
            }

            var search = await query
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType,
                            RegNumber = v.RegNumber,
                            ArrivalDate = v.ArrivalDate,
                            ParkingSpot = v.ParkingSpot
                        }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "No vehicles found";
            }

            else
            {
                TempData["SearchSuccess"] = "Search was successful";
            }

            return View("Index", search);
        }

        [HttpGet]
        public async Task<IActionResult> ShowAll()
        {
            int availableSpaces = MaxParkingSpaces - _context.Vehicle.Count();
            ViewBag.AvailableSpaces = availableSpaces;

            var query = _context.Vehicle.AsQueryable();
            var search = await query
                      .Select(v => new VehicleViewModel
                      {
                          Id = v.Id,
                          VehicleType = v.VehicleType,
                          RegNumber = v.RegNumber,
                          ArrivalDate = v.ArrivalDate,
                          ParkingSpot = v.ParkingSpot
                      }).ToListAsync();

            if (search.Count == 0)
            {
                TempData["SearchFail"] = "There is no vehicles in the system";
            }

            else
            {
                TempData["SearchSuccess"] = "Showing all vehicles was successful";
            }

            return View("Index", search);
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            int availableSpaces = MaxParkingSpaces - _context.Vehicle.Count();
            ViewBag.AvailableSpaces = availableSpaces;


            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
            }

            var vehicles = await _context.Vehicle
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    VehicleType = v.VehicleType,
                    RegNumber = v.RegNumber,
                    ArrivalDate = v.ArrivalDate,
                    ParkingSpot = v.ParkingSpot
                }).ToListAsync();
            
            double totalRevenue = 0;
            for ( var i = 0; i < vehicles.Count; i++)
            {
                ReceiptViewModel receipt = new ReceiptViewModel();

                receipt.ArrivalDate = vehicles[i].ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();
                totalRevenue = totalRevenue + receipt.Price;
            }
            string formattedRevenue = totalRevenue.ToString("#,##0.00");
            TempData["TotalRevenue"] = $"{formattedRevenue}";

            return View(vehicles);
        }

        public async Task<IActionResult> GarageView()
        {
            int availableSpaces = MaxParkingSpaces - _context.Vehicle.Count();
            ViewBag.AvailableSpaces = availableSpaces;


            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
            }

            var vehicles = await _context.Vehicle
                .Select(v => new VehicleViewModel
                {
                    Id = v.Id,
                    VehicleType = v.VehicleType,
                    RegNumber = v.RegNumber,
                    ArrivalDate = v.ArrivalDate,
                    ParkingSpot = v.ParkingSpot
                }).ToListAsync();

            return View(vehicles);
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                ReceiptViewModel receipt = new ReceiptViewModel();

                //receipt.Id = vehicle.Id;
                //receipt.RegNumber = vehicle.RegNumber;
                receipt.ArrivalDate = vehicle.ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();

                var price = receipt.Price;
                TempData["Price"] = $"{price}";
                return View(vehicle);
            }
        }

        // GET: Vehicles/Create
        [HttpGet]
        public IActionResult Create()
        {
            int totalVehicles = _context.Vehicle.Count();

            if (totalVehicles >= MaxParkingSpaces)
            {
                TempData["Message"] = "Parking is full. Cannot check in new vehicle.";
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VehicleType,RegNumber,Color,Brand,VehicleModel,NrOfWheels")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                if (_context.Vehicle.Any(v => v.RegNumber == vehicle.RegNumber))
                {
                    ModelState.AddModelError("RegNumber", "A vehicle with this registration number already exists.");
                    return View(vehicle);
                }

                int totalVehicles = _context.Vehicle.Count();

                if (totalVehicles >= MaxParkingSpaces)
                {
                    TempData["Message"] = "Parking is full. Cannot check in new vehicle.";
                    return RedirectToAction(nameof(Index));
                }

                string parkingSpot = FindParkingSpot();

                vehicle.ParkingSpot = parkingSpot;
                vehicle.ArrivalDate = DateTime.Now;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Vehicle is checked in";
                return RedirectToAction(nameof(Index));
            }

            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,RegNumber,Color,Brand,VehicleModel,NrOfWheels")] Vehicle updatedVehicle)
        {
            if (id != updatedVehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingVehicle = await _context.Vehicle.FindAsync(id);

                    existingVehicle.VehicleType = updatedVehicle.VehicleType;
                    existingVehicle.RegNumber = updatedVehicle.RegNumber;
                    existingVehicle.Color = updatedVehicle.Color;
                    existingVehicle.Brand = updatedVehicle.Brand;
                    existingVehicle.VehicleModel = updatedVehicle.VehicleModel;
                    existingVehicle.NrOfWheels = updatedVehicle.NrOfWheels;

                    _context.Update(existingVehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(updatedVehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "Vehicle is updated";
                return RedirectToAction(nameof(Index));
            }

            return View(updatedVehicle);
        }

        // GET: Vehicles/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FirstOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                ReceiptViewModel receipt = new ReceiptViewModel();

                receipt.ArrivalDate = vehicle.ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();

                var price = receipt.Price;
                TempData["Price"] = $"{price}";
                TempData["Message"] = "Vehicle is updated";
                return View(vehicle);
            }
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicle.FindAsync(id);

            if (vehicle != null)
            {
                var spotNumber = vehicle.ParkingSpot;
                ParkingSpots[int.Parse(spotNumber) - 1] = null;

                _context.Vehicle.Remove(vehicle);
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Vehicle is checked out";
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }
    }
}
