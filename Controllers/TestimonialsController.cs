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
    public class TestimonialsController : Controller
    {
        private readonly ModelContext _context;

        public TestimonialsController(ModelContext context)
        {
            _context = context;
        }

         
        // GET: Testimonials
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Testimonials.Include(t => t.User).ThenInclude(u => u.Profile);
            return View(await modelContext.ToListAsync());
        }


        // GET: Testimonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.User).ThenInclude(u=>u.Profile)
                .FirstOrDefaultAsync(m => m.Testimonialid == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // GET: Testimonials/Create
        public IActionResult Create()
        {
             var members = _context.Users
                .Where(u => u.Roleid == 3)
                .Select(u => new
                {
                    u.Userid,  
                    FullName = u.Profile != null
                        ? $"{u.Profile.Fname} {u.Profile.Lname}"
                        : "No Name"  
                })
                .ToList();

             ViewData["Userid"] = new SelectList(members, "Userid", "FullName");

            return View();
        }

        // POST: Testimonials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Testimonialid,Content,Userid")] Testimonial testimonial)
        {
            if (ModelState.IsValid)
            {
                 _context.Add(testimonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

             var members = _context.Users
                .Where(u => u.Roleid == 3)
                .Select(u => new
                {
                    u.Userid,
                    FullName = u.Profile != null
                        ? $"{u.Profile.Fname} {u.Profile.Lname}"
                        : "No Name"
                })
                .ToList();

            ViewData["Userid"] = new SelectList(members, "Userid", "FullName", testimonial.Userid);

            return View(testimonial);
        }


        // GET: Testimonials/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            var members = _context.Users
                .Where(u => u.Roleid == 3)
                .Select(u => new
                {
                    u.Userid,
                    FullName = u.Profile != null
                        ? $"{u.Profile.Fname} {u.Profile.Lname}"
                        : "No Name" 
                })
                .ToList();

            ViewData["Userid"] = new SelectList(members, "Userid", "FullName", testimonial.Userid);
            return View(testimonial);
        }

        // POST: Testimonials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Testimonialid,Content,Userid")] Testimonial testimonial)
        {
            if (id != testimonial.Testimonialid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testimonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestimonialExists(testimonial.Testimonialid))
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

            var members = _context.Users
                .Where(u => u.Roleid == 3)
                .Select(u => new
                {
                    u.Userid,
                    FullName = u.Profile != null
                        ? $"{u.Profile.Fname} {u.Profile.Lname}"
                        : "No Name"
                })
                .ToList();

            ViewData["Userid"] = new SelectList(members, "Userid", "FullName", testimonial.Userid);
            return View(testimonial);
        }

        // GET: Testimonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Testimonials == null)
            {
                return NotFound();
            }

            var testimonial = await _context.Testimonials
                .Include(t => t.User)
                .ThenInclude(u=>u.Profile)
                .FirstOrDefaultAsync(m => m.Testimonialid == id);
            if (testimonial == null)
            {
                return NotFound();
            }

            return View(testimonial);
        }

        // POST: Testimonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Testimonials == null)
            {
                return Problem("Entity set 'ModelContext.Testimonials'  is null.");
            }
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                _context.Testimonials.Remove(testimonial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestimonialExists(decimal id)
        {
          return (_context.Testimonials?.Any(e => e.Testimonialid == id)).GetValueOrDefault();
        }
    }
}
