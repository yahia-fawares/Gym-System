using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymSystem2.Models;

namespace GymSystem2.Controllers
{
    public class PlansController : Controller
    {
        private readonly ModelContext _context;

        public PlansController(ModelContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            var plans = await _context.Plans
            .Include(p => p.Trainer)
            .ThenInclude(t => t.Profile)
            .ToListAsync();
            if (plans == null || !plans.Any())
            {
                ViewBag.NoPlans = true;
            }

            return View(plans);
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Trainer) // Include العلاقة مع Trainer
                .ThenInclude(u => u.Profile) // Include العلاقة مع Profile
                .FirstOrDefaultAsync(m => m.Planid == id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }


        // GET: Plans/Create
        public IActionResult Create()
        {
            var trainers = _context.Users
                .Select(user => new
                {
                    user.Userid,
                    Fname = user.Profile != null ? user.Profile.Fname : "No Name"
                })
                .ToList();

            ViewData["Trainerid"] = new SelectList(trainers, "Userid", "Fname");

            return View();
        }



        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Planid,Planname,Description,Price,Trainerid")] Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Trainerid"] = new SelectList(_context.Users, "Userid", "Profile.Fname", plan.Trainerid);
            return View(plan);
        }


        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            var trainers = _context.Users
                .Select(user => new
                {
                    user.Userid,
                    Fname = user.Profile != null ? user.Profile.Fname : "No Name"
                })
                .ToList();

            ViewData["Trainerid"] = new SelectList(trainers, "Userid", "Fname", plan.Trainerid);
            return View(plan);
        }


        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Planid,Planname,Description,Price,Trainerid")] Plan plan)
        {
            if (id != plan.Planid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.Planid))
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

            var trainers = _context.Users
                .Select(user => new
                {
                    user.Userid,
                    Fname = user.Profile != null ? user.Profile.Fname : "No Name"
                })
                .ToList();

            ViewData["Trainerid"] = new SelectList(trainers, "Userid", "Fname", plan.Trainerid);
            return View(plan);
        }


        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Trainer) 
                .ThenInclude(u => u.Profile) 
                .FirstOrDefaultAsync(m => m.Planid == id);

            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Plans == null)
            {
                return Problem("Entity set 'ModelContext.Plans'  is null.");
            }
            var plan = await _context.Plans.FindAsync(id);
            if (plan != null)
            {
                _context.Plans.Remove(plan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanExists(decimal id)
        {
          return (_context.Plans?.Any(e => e.Planid == id)).GetValueOrDefault();
        }
    }
}
