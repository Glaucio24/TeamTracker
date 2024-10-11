using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
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

       

        //        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        //        }


        //    public async IActionResult Statistics()
        //    {
        //        return View();
        //    }

        //    // GET: Statistics
        //    public async Task<IActionResult> Index()
        //    {
        //        return View(await _context.Statistics.ToListAsync());
        //    }
    }
}
