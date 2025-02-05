using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymSystem2.Models;
using System.Numerics;

namespace GymSystem2.Controllers
{
    public class SubscriptionsController : Controller
    {
        private readonly ModelContext _context;

        public SubscriptionsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            var modelContext = await _context.Subscriptions
                .Include(s => s.Member) // تضمين بيانات الـ Member
                .ThenInclude(m => m.Profile) // تضمين الـ Profile الخاص بـ Member
                .Include(s => s.Plan) // تضمين بيانات الخطة
                .ToListAsync();
            return View( modelContext);
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member) // تضمين بيانات الـ Member
                .ThenInclude(m => m.Profile) // تضمين الـ Profile الخاص بـ Member
                .Include(s => s.Plan) // تضمين بيانات الخطة
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        //Get//Create
        public IActionResult Create()
        {
            // تحميل المستخدمين الذين لديهم Roleid == 3 مع تضمين بيانات الـ Profile
            ViewData["Memberid"] = new SelectList(_context.Users
                .Where(u => u.Roleid == 3)
                .Include(u => u.Profile), "Userid", "Profile.Fname");

            // تحميل الخطط المتاحة
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planname");

            return View();
        }



        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subscriptionid,Fromdate,Todate,Memberid,Planid")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

             
            ViewData["Memberid"] = new SelectList(_context.Users
                .Where(u => u.Roleid == 3)
                .Include(u => u.Profile), "Userid", "Profile.Fname", subscription.Memberid);

            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planname", subscription.Planid);

            return View(subscription);
        }



        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["Memberid"] = new SelectList(_context.Users
               .Where(u => u.Roleid == 3)
               .Include(u => u.Profile), "Userid", "Profile.Fname");
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planname");
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Subscriptionid,Fromdate,Todate,Memberid,Planid")] Subscription subscription)
        {
            if (id != subscription.Subscriptionid)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                 foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }

                
                ViewData["Memberid"] = new SelectList(
                    _context.Users
                    .Where(u => u.Roleid == 3)
                    .Include(u => u.Profile),
                    "Userid",
                    "Profile.Fname");

                ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planname");
                return View(subscription);
            }

            try
            {
                // تحديث البيانات وحفظها
                _context.Update(subscription);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsSubscriptionExists(subscription.Subscriptionid))
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

        // تحقق من وجود الاشتراك
        private bool IsSubscriptionExists(decimal id)
        {
            return _context.Subscriptions.Any(e => e.Subscriptionid == id);
        }


        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member) // تضمين بيانات الـ Member
                .ThenInclude(m => m.Profile) // تضمين الـ Profile الخاص بـ Member
                .Include(s => s.Plan) // تضمين بيانات الخطة
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Subscriptions == null)
            {
                return Problem("Entity set 'ModelContext.Subscriptions'  is null.");
            }
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(decimal id)
        {
          return (_context.Subscriptions?.Any(e => e.Subscriptionid == id)).GetValueOrDefault();
        }
    }
}
