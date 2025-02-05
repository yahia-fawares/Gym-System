using GymSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GymSystem2.Controllers
{
    public class AdminController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }





        public async Task<IActionResult> Index()
        {
            var monthlyData = _context.Subscriptions
                .Where(s => s.Fromdate.Year == DateTime.Now.Year)
                .GroupBy(s => s.Fromdate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                }).ToList();

            var annualData = _context.Subscriptions
                .GroupBy(s => s.Fromdate.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Count = g.Count()
                }).ToList();

            // تسلسل البيانات إلى JSON
            var monthlyDataJson = JsonConvert.SerializeObject(monthlyData);
            var annualDataJson = JsonConvert.SerializeObject(annualData);

        
            var model = new DashboardViewModel
            {
                UsersCount = _context.Users.Count(),
                PlanCount = _context.Plans.Count(),
                TotalPrice = _context.Plans.Sum(x => x.Price),
                SubscriptionsCount = _context.Subscriptions.Count(),
                Users = await _context.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Plans)
                    .Include(u => u.Subscriptions)
                    .Include(u => u.Card)
                    .Include(u => u.Testimonials)
                    .ToListAsync(),
                MonthlyData = monthlyDataJson,
                AnnualData = annualDataJson
            };

            return View(model);
        }





        public IActionResult AdminRegister()
        {
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminRegister([Bind("Profile,Userid,Email,Password,Roleid")] User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .Include(u => u.Profile) 
                    .FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "This email is already registered.");
                    ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
                    return View(user);
                }

                if (user.Profile.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath; 
                    string fileName = Guid.NewGuid().ToString() + "_" + user.Profile.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await user.Profile.ImageFile.CopyToAsync(fileStream);
                    }

                    user.Profile.Imagepath = fileName;
                }

                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Admin");
            }

            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }


        public IActionResult Search()
        {
             var result = _context.Subscriptions.Include(x => x.Plan).Include(x => x.Member).ThenInclude(m=>m.Profile).ToList();

 
            return View(result);
        }

        [HttpPost]
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            var result = _context.Subscriptions.Include(x => x.Plan).Include(x => x.Member).ThenInclude(m=>m.Profile).ToList();

            if (startDate == null && endDate == null)
            {
                 return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                result = result.Where(x => x.Fromdate.Date >= startDate).ToList();
 
                return View(result);
            }
            else if (startDate == null && endDate != null)
            {
                result = result.Where(x => x.Todate.Date <= endDate).ToList();
 
                return View(result);
            }
            else
            {
                result = result.Where(x => x.Fromdate.Date >= startDate && x.Todate.Date <= endDate).ToList();
 
                return View(result);
            }
        }

        public async Task<IActionResult> Approve(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.IsApprove = true;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Testimonials");
        }

        public async Task<IActionResult> Reject(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.IsApprove = false;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Testimonials");
        }
    }
}
