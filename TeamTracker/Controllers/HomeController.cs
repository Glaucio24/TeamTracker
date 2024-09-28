using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TeamTracker.Data;
using TeamTracker.Models;
using TeamTracker.ModelView;

namespace TeamTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult Statistics()
        {
            var dashboard = new Dashboard();

            // Fetch departments with their related employees
            var departments = _context.Departments
                .Include(d => d.Employees)
                .ToList();

            // Fetch locations with their related employees
            var locations = _context.Locations
                .Include(l => l.Employees)
                .ToList();

            // Populate dashboard counts
            dashboard.TotalDepartments = departments.Count;
            dashboard.TotalLocations = locations.Count;
            dashboard.TotalEmployees = _context.Employees.Count();

            // Populate department data
            dashboard.DepartmentNames = departments.Select(d => d.Name).ToList();
            dashboard.EmployeesPerDepartment = departments.Select(d => d.Employees.Count).ToList();

            // Populate location data
            dashboard.LocationNames = locations.Select(l => l.Name).ToList();
            dashboard.EmployeesPerLocation = locations.Select(l => l.Employees.Count).ToList();

            // Prepare chart data for JavaScript
            ViewBag.EmployeesByDepartmentDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(
                departments.Select(d => new {
                    label = d.Name,
                    y = d.Employees.Count
                }).ToList());

            ViewBag.EmployeesByLocationDataPoints = Newtonsoft.Json.JsonConvert.SerializeObject(
                locations.Select(l => new {
                    label = l.Name,
                    y = l.Employees.Count
                }).ToList());

            return View(dashboard);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
