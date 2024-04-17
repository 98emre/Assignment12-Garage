using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment12_Garage.Data;
using Assignment12_Garage.Models;

namespace Assignment12_Garage.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Assignment12_GarageContext _context;

        public VehiclesController(Assignment12_GarageContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Sort(string regNumber, string color, string brand)
        {
            var query = _context.Vehicle.AsQueryable();

            if (!string.IsNullOrEmpty(regNumber))
            {
                query = query.Where(v => v.RegNumber.Equals(regNumber.ToUpper().Trim()));
            }

            if (!string.IsNullOrEmpty(color))
            {
                query = query.Where(v => v.Color.Equals(color, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(v => v.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
            }

            var search = await query.ToListAsync();

            if (search.Count == 0)
            {
                TempData["Message"] = "No vehicles found matching the specified criteria.";
                return RedirectToAction("Index");
            }

            return View("Index", search);
        }


        public async Task<IActionResult> Filter(string regNumber, string color, string brand)
        {
            try
            {
                if (string.IsNullOrEmpty(regNumber) && string.IsNullOrEmpty(color) && string.IsNullOrEmpty(brand))
                {
                    TempData["Message"] = "Please provide input for at least one search criteria.";
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

                var search = await query.ToListAsync();

                if (search.Count == 0)
                {
                    TempData["Message"] = "No vehicles found";
                    return RedirectToAction("Index");
                }

                return View("Index", search);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "An error occurred while processing the request.";
                return RedirectToAction("Index");
            }
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vehicle.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
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
                if(_context.Vehicle.Any(v => v.RegNumber == vehicle.RegNumber))
                {
                    ModelState.AddModelError("RegNumber", "A vehicle with this registration number already exists.");
                    return View(vehicle);
                }
                vehicle.ArrivalDate = DateTime.Now;
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(updatedVehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicle.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }
    }
}
