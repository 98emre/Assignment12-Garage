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
                    ParkingSpots.Add("Empty");
                }
            }
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

            double totalRevenue = 0;
            for (var i = 0; i < parkedVehicles.Count; i++)
            {
                ReceiptViewModel receipt = new ReceiptViewModel();

                receipt.ArrivalDate = parkedVehicles[i].ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();
                totalRevenue = totalRevenue + receipt.Price;
            }

            ViewBag.TotalRevenue = totalRevenue.ToString("#,##0.00");
            ViewBag.VehicleType = vehicleTypeCount;
            ViewBag.TotalWheels = totalWheels;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            var vehicles = await _context.Vehicle.ToListAsync();

            if (vehicles.Count() == 0)
            {
                TempData["SortOnEmptyList"] = "The list is empty";
                return RedirectToAction(nameof(Index)); 
            }

            switch (sortOrder)
            {
                case "vehicleType":
                    vehicles = vehicles.OrderBy(v => v.VehicleType).ToList();
                    TempData["Sort"] = "Vehicle type sort was done";
                    break;
                case "regNumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber).ToList();
                    TempData["Sort"] = "Registration number sort was done";
                    break;
                case "arrivalDate":
                    vehicles = vehicles.OrderBy(v => v.ArrivalDate).ToList();
                    TempData["Sort"] = "Arrival Date sort was done";
                    break;
                case "parkingSpot":
                    vehicles = vehicles.OrderBy(v => int.TryParse(v.ParkingSpot, out int spot) ? spot : int.MaxValue).ToList();
                    TempData["Sort"] = "Parking Spot sort was done";
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Id).ToList();
                    break;
            }

            var sortedVehicles = vehicles
                        .Select(v => new VehicleViewModel
                        {
                            Id = v.Id,
                            VehicleType = v.VehicleType,
                            RegNumber = v.RegNumber,
                            ArrivalDate = v.ArrivalDate,
                            ParkingSpot = v.ParkingSpot
                        }).ToList();

            return View("Index", sortedVehicles);
        }

        [HttpGet]
        public async Task<IActionResult> Filter(string regNumber, string color, string brand)
        {
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
            int spaces = _context.Vehicle
                .Where(v => v.ParkingSpot != "Empty")
                .Select(v => v.ParkingSpot)
                .Distinct()
                .Count();

            int availableSpaces = MaxParkingSpaces - spaces;
            ViewBag.AvailableVehicels = _context.Vehicle.ToList().Count();

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

        public IActionResult GarageView()
        {
            int spaces = _context.Vehicle
                 .Where(v => v.ParkingSpot != "Empty")
                 .Select(v => v.ParkingSpot)
                 .Distinct()
                 .Count();

            int availableSpaces = MaxParkingSpaces - spaces;

            ViewBag.AvailableSpaces = availableSpaces;


            var vehicleList = new List<VehicleViewModel>();
            int index = 0;

            foreach (var spot in ParkingSpots)
            {
                
              index++;

               var vehicleOnSpot = _context.Vehicle.ToList().FirstOrDefault(v => v.ParkingSpot == spot);
                
                if (vehicleOnSpot != null)
                {
                  vehicleList.Add(new VehicleViewModel
                  {
                      Id = vehicleOnSpot.Id,
                      VehicleType = vehicleOnSpot.VehicleType,
                      RegNumber = vehicleOnSpot.RegNumber,
                      ArrivalDate = vehicleOnSpot.ArrivalDate,
                      ParkingSpot = vehicleOnSpot.ParkingSpot
                  });
                }
                
                else
                {
                    vehicleList.Add(new VehicleViewModel
                    {
                        ParkingSpot = index.ToString()
                      });
                }              
            }

            return View(vehicleList);
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

                receipt.ArrivalDate = vehicle.ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();

                var price = receipt.Price.ToString("#,##0.00"); ;
                TempData["Price"] = $"{price}";
                return View(vehicle);
            }
        }

        // GET: Vehicles/Create
        [HttpGet]
        public IActionResult Create()
        {
            int spaces = _context.Vehicle
                              .Where(v => v.ParkingSpot != "Empty")
                              .Select(v => v.ParkingSpot)
                              .Distinct()
                              .Count();

            if (spaces >= MaxParkingSpaces)
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

                int spaces = _context.Vehicle
                              .Where(v => v.ParkingSpot != "Empty")
                              .Select(v => v.ParkingSpot)
                              .Distinct()
                              .Count();

                if (spaces >= MaxParkingSpaces)
                {
                    TempData["Message"] = "Parking is full. Cannot check in new vehicle.";
                    return RedirectToAction(nameof(Index));
                }

                string parkingSpot = FindParkingSpot();

                if (vehicle.VehicleType == VehicleType.Motorcycle)
                {
                    // Initiate ParkingHelper class
                    ParkingHelper parkingHelper = new ParkingHelper(_context);
                    string lookForSpot = parkingHelper.TryFindMotorcycleSpot();
                    // If lookForSpot is not just an empty string. Then we found a good spot.
                    if (lookForSpot != "")
                    {
                        parkingSpot = parkingHelper.TryFindMotorcycleSpot();
                    }
                }

                vehicle.ParkingSpot = parkingSpot;
                vehicle.ArrivalDate = DateTime.Now;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Vehicle with registration number ({vehicle.RegNumber}) is check in";
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

                    TempData["Message"] = $"Vehicle with registration number ({existingVehicle.RegNumber}) is updated";

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

                receipt.RegNumber = vehicle.RegNumber;
                receipt.ArrivalDate = vehicle.ArrivalDate;
                receipt.CheckoutDate = DateTime.Now;
                receipt.CalculateTotalParkingHours();
                receipt.CalculatePrice();

                TempData["Price"] = receipt.Price.ToString("#,##0.00");

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
                ParkingSpots[int.Parse(spotNumber) - 1] = "Empty";

                ReceiptViewModel receiptViewModel = new ReceiptViewModel();

                receiptViewModel.VehicleType = vehicle.VehicleType;
                receiptViewModel.RegNumber = vehicle.RegNumber;
                receiptViewModel.ArrivalDate = vehicle.ArrivalDate;
                receiptViewModel.CheckoutDate = DateTime.Now;
                receiptViewModel.CalculateTotalParkingHours();
                receiptViewModel.CalculatePrice();

                TempData["Price"] = receiptViewModel.Price.ToString("#,##0.00");

                _context.Vehicle.Remove(vehicle);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Vehicle with registration number ({vehicle.RegNumber}) is checkout";

                return View("Receipt", receiptViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }

        private string FindParkingSpot()
        {
            for (int i = 0; i < ParkingSpots.Count; i++)
            {
                if (ParkingSpots[i] == "Empty")
                {
                    return (i + 1).ToString();
                }
            }

            return "Empty";
        }
    }
}
