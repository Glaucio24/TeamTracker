
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamTracker.Data;
using TeamTracker.ModelView;
using TeamTracker.Models;

namespace TeamTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var statistics = new Statistics
            {
                TotalEmployees = _context.Employees.Count(),
                TotalDepartments = _context.Departments.Count(),
                TotalLocations = _context.Locations.Count()
            };

            // Fetch departments with their related employees
            var departments = _context.Departments
                .Include(d => d.Employees) // Ensure Employees is a navigation property
                .ToList();

            // Fetch locations with their related employees
            var locations = _context.Locations
                .Include(l => l.Employees) // Ensure Employees is a navigation property
                .ToList();

            // Populate department data
            statistics.DepartmentNames = departments.Select(d => d.Name).ToList();
            statistics.EmployeesPerDepartment = departments.Select(d => d.Employees.Count).ToList();

            // Populate location data
            statistics.LocationNames = locations.Select(l => l.Name).ToList();
            statistics.EmployeesPerLocation = locations.Select(l => l.Employees.Count).ToList();

            return View(statistics); // Pass the model to the view
        }

        public IActionResult HR()
        {
            var statistics = new Statistics();

            // Fetch departments with their related employees
            var departments = _context.Departments
                .Include(d => d.Employees) // Ensure Employees is a navigation property
                .ToList();

            // Fetch locations with their related employees
            var locations = _context.Locations
                .Include(l => l.Employees) // Ensure Employees is a navigation property
                .ToList();

            // Store the employee count once
            var totalEmployees = _context.Employees.Count();

            // Populate dashboard counts
            statistics.TotalDepartments = departments.Count;
            statistics.TotalLocations = locations.Count;
            statistics.TotalEmployees = totalEmployees;

            // Populate department data
            statistics.DepartmentNames = departments.Select(d => d.Name).ToList();
            statistics.EmployeesPerDepartment = departments.Select(d => d.Employees.Count).ToList(); // Fix method invocation

            // Populate location data
            statistics.LocationNames = locations.Select(l => l.Name).ToList();
            statistics.EmployeesPerLocation = locations.Select(l => l.Employees.Count).ToList(); // Fix method invocation

            // Prepare chart data for JavaScript
            ViewBag.EmployeesByDepartmentDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(
                departments.Select(d => new
                {
                    label = d.Name,
                    y = d.Employees.Count // Fix method invocation
                }).ToList());

            ViewBag.EmployeesByLocationDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(
                locations.Select(l => new
                {
                    label = l.Name,
                    y = l.Employees.Count // Fix method invocation
                }).ToList());

            return View(statistics);
        }
    }
}
