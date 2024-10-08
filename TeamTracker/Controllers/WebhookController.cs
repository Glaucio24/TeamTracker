using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TeamTracker.Data;
using TeamTracker.Models;

namespace TeamTracker.Controllers
{
    [Route("api/webhook")]
    public class WebhookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WebhookController(ApplicationDbContext context)
        {
            _context = context;
        }

       
    }
}
