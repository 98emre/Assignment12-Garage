﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment12_Garage.Data;
using Assignment12_Garage.Models;
using Assignment12_Garage.Models.ViewModels;

namespace Assignment12_Garage.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Assignment12_GarageContext _context;

        public VehiclesController(Assignment12_GarageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Sort(string sortOrder)
        {
            var vehicles = _context.Vehicle.AsQueryable();

            switch (sortOrder)
            {
                case "vehicleType":
                    vehicles = vehicles.OrderBy(v => v.VehicleType);
                    break;
                case "regNumber":
                    vehicles = vehicles.OrderBy(v => v.RegNumber);
                    break;
                case "color":
                    vehicles = vehicles.OrderBy(v => v.Color);
                    break;
                case "brand":
                    vehicles = vehicles.OrderBy(v => v.Brand);
                    break;
                case "vehicleModel":
                    vehicles = vehicles.OrderBy(v => v.VehicleModel);
                    break;
                case "nrOfWheels":
                    vehicles = vehicles.OrderBy(v => v.NrOfWheels);
                    break;
                case "arrivalDate":
                    vehicles = vehicles.OrderBy(v => v.ArrivalDate);
                    break;
                default:
                    vehicles = vehicles.OrderBy(v => v.Id); 
                    break;
            }

            var sortedVehicles = await vehicles.ToListAsync();

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

                var search = await query.ToListAsync();

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
            var search = await query.ToListAsync();

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
            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
            }

            return View(await _context.Vehicle.ToListAsync());
        }

        public async Task<IActionResult> VehicleOverview()
        {
            var model = _context.Vehicle.Select(e => new VehicleViewModel
            {
                Id = e.Id,
                VehicleType = e.VehicleType,
                RegNumber = e.RegNumber,
                ArrivalDate = e.ArrivalDate,
            });
            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
            }

            return View("VehicleOverview",await model.ToListAsync());
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

            TempData["Message"] = "Vehicle is updated";
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

            TempData["Message"] = "Vehicle is checked out";
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicle.Any(e => e.Id == id);
        }
    }
}
