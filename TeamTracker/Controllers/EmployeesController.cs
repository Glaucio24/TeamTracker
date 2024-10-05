using System;
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
using TeamTracker.Enums;
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
        public async Task<IActionResult> Index(int? EmployeesId, string searchString, int page = 1, int pageSize = 3)
        {
            // Initialize the employee query to properly load Depts and locations          
            var employees = _context.Employees
             .Include(e => e.Departments)
              .Include(e => e.Locations)  
              .AsQueryable();

            // Handle search by name
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                                  e.LastName.ToLower().Contains(searchString.ToLower()));
            }

            // Handle filter by specific employee from the dropdown
            if (EmployeesId.HasValue && EmployeesId != 0)
            {
                employees = employees.Where(e => e.Id == EmployeesId);
            }

            // Order employees by FullName (or FirstName and LastName separately)
            employees = employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName);

            // Get the total number of employees after filtering
            var totalEmployees = await employees.CountAsync();

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);

            // Get the employees for the current page
            var paginatedEmployees = await employees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Populate dropdown for employees
            ViewBag.EmployeesId = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", EmployeesId);

            // Pass pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewData["SearchString"] = searchString; // Maintain the search string
            ViewData["EmployeesId"] = EmployeesId; // Maintain the employee ID

            // Return the filtered and ordered employees to the view
            return View(paginatedEmployees);
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

             // Create a SelectList for the EmploymentStatus enum
             ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(EmploymentStatus)));                        

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

            // Create a SelectList for the EmploymentStatus enum and set the current value as selected
            ViewBag.StatusList = new SelectList(Enum.GetValues(typeof(EmploymentStatus)), employee.Status);


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
