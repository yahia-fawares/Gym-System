using GymSystem2.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GymSystem2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;



        public HomeController(ILogger<HomeController> logger, ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _logger = logger;
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task<IActionResult> Index()
        {
            

            var plans = await _context.Plans
            .Include(p => p.Trainer)
            .ThenInclude(t => t.Profile)
            .ToListAsync();
            ViewBag.plans = plans;
            return View();
        }
      

        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Classes()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            var testimonials = _context.Testimonials
                .Include(t => t.User)
                .ThenInclude(u=>u.Profile)
                .Where(t => t.IsApprove)
                .ToList();

            return View(testimonials);
        }
        public IActionResult Services()
        {
            return View();
        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
