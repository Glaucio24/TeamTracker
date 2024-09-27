﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using TeamTracker.Data;
using TeamTracker.Models;
using TeamTracker.Services.Interfaces;

namespace TeamTracker.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;

        public EmployeesController(ApplicationDbContext context, UserManager<AppUser> userManager, IImageService imageService )
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
        }

        // GET: Employees
        [Authorize]
        public async Task<IActionResult> Index(int employeeId)
        {
            // Initialize the employee list
            var employees = new List<Employee>();

            // If no specific employeeId is selected, return all employees
            if (employeeId == 0)
            {
                employees = await _context.Employees
                    .Include(e => e.Departments)
                    .Include(e => e.Locations)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();
            }
            else
            {
                // Filter by employeeId
                employees = await _context.Employees
                    .Include(e => e.Departments)
                    .Include(e => e.Locations)
                    .Where(e => e.Id == employeeId)
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();
            }

            // Get department and location lists for dropdown filters
            var departments = await _context.Departments.ToListAsync();
            var locations = await _context.Locations.ToListAsync();

            // Pass the department and location lists to the View using ViewData
            ViewData["DepartmentId"] = new SelectList(departments, "Id", "Name");
            ViewData["LocationId"] = new SelectList(locations, "Id", "Name");

            return View(employees);
        }





        public IActionResult SearchEmployees(string searchString, int? departmentId, int? locationId)
        {
            // Initialize an empty employee list
            var employees = _context.Employees
                .Include(e => e.Departments)
                .Include(e => e.Locations)
                .AsQueryable();

            // Filter by search string (if provided)
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                                  e.LastName.ToLower().Contains(searchString.ToLower()));
            }

            // Filter by department if a departmentId is provided
            if (departmentId.HasValue && departmentId != 0)
            {
                employees = employees.Where(e => e.Departments.Any(d => d.Id == departmentId));
            }

            // Filter by location if a locationId is provided
            if (locationId.HasValue && locationId != 0)
            {
                employees = employees.Where(e => e.Locations.Any(l => l.Id == locationId));
            }

            return View(nameof(Index), employees.ToList());
        }




        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            string AppUserId = _userManager.GetUserId(User);

            // Populate ViewBag with employees
            ViewBag.LocationList = new SelectList(_context.Locations, "Id", "Name");

            // Populate ViewBag with departments
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name");

            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,Status,HireDate,ImageData,ImageType,ImageFile")] Employee employee, int[] locationIds, int[] departmentIds)
        {
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                // Handle image upload
                if (employee.ImageFile != null)
                {
                    // Convert file to byte array (if needed for storage in the database)
                    employee.ImageData = await _imageService.ConvertFileToByteArrayAsync(employee.ImageFile);
                    employee.ImageType = employee.ImageFile.ContentType;

                    // Store image file and save the path
                    var fileName = Path.GetFileName(employee.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save the file to the specified path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await employee.ImageFile.CopyToAsync(stream);
                    }

                    // Set the ImagePath property (relative to wwwroot)
                    employee.ImagePath = "/img/" + fileName;
                }

                // Associate selected locations
                if (locationIds != null)
                {
                    foreach (var locationId in locationIds)
                    {
                        var location = await _context.Locations.FindAsync(locationId);
                        if (location != null)
                        {
                            employee.Locations.Add(location);
                        }
                    }
                }

                // Associate selected departments
                if (departmentIds != null)
                {
                    foreach (var departmentId in departmentIds)
                    {
                        var department = await _context.Departments.FindAsync(departmentId);
                        if (department != null)
                        {
                            employee.Departments.Add(department);
                        }
                    }
                }

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Populate ViewBag for locations and departments in case of failure
            ViewBag.LocationList = new SelectList(_context.Locations, "Id", "Name");
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name");
            return View(employee);
        }



        // GET: Employees/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // Populate ViewBag for locations and departments
            ViewBag.LocationList = new SelectList(_context.Locations, "Id", "Name", employee.Locations.Select(l => l.Id));
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name", employee.Departments.Select(d => d.Id));

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Status,HireDate,ImageData,ImageType,ImageFile")] Employee employee, int[] locationIds, int[] departmentIds)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEmployee = await _context.Employees
                        .Include(e => e.Locations) // Include locations if necessary
                        .Include(e => e.Departments) // Include departments if necessary
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingEmployee != null)
                    {
                        // Update properties
                        existingEmployee.FirstName = employee.FirstName;
                        existingEmployee.LastName = employee.LastName;
                        existingEmployee.Email = employee.Email;
                        existingEmployee.PhoneNumber = employee.PhoneNumber;
                        existingEmployee.Status = employee.Status;

                        // Update the image only if a new one is provided
                        if (employee.ImageFile != null)
                        {
                            existingEmployee.ImageData = await _imageService.ConvertFileToByteArrayAsync(employee.ImageFile);
                            existingEmployee.ImageType = employee.ImageFile.ContentType;
                        }

                        // Clear existing associations
                        existingEmployee.Locations.Clear();
                        existingEmployee.Departments.Clear();

                        // Associate new locations
                        if (locationIds != null)
                        {
                            foreach (var locationId in locationIds)
                            {
                                var location = await _context.Locations.FindAsync(locationId);
                                if (location != null)
                                {
                                    existingEmployee.Locations.Add(location);
                                }
                            }
                        }

                        // Associate new departments
                        if (departmentIds != null)
                        {
                            foreach (var departmentId in departmentIds)
                            {
                                var department = await _context.Departments.FindAsync(departmentId);
                                if (department != null)
                                {
                                    existingEmployee.Departments.Add(department);
                                }
                            }
                        }

                        // Save changes
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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

            ViewBag.LocationList = new SelectList(_context.Locations, "Id", "Name");
            ViewBag.DepartmentList = new SelectList(_context.Departments, "Id", "Name");
            return View(employee);
        }

        // GET: Employees1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
