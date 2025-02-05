using GymSystem2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymSystem2.Controllers
{
   
    public class MemberController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public MemberController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        public async Task<IActionResult> MemberDashboard()
        {
            // الحصول على معرف العضو من Claims
            var memberId = User.FindFirst("Userid")?.Value;


            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            // جلب بيانات العضو من قاعدة البيانات
            var member = await _context.Users
                .Include(u => u.Profile) // تضمين بيانات الملف الشخصي
                .Include(u => u.Subscriptions) // تضمين بيانات الاشتراكات
                .FirstOrDefaultAsync(u => u.Userid == decimal.Parse(memberId));

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        public IActionResult SubmitFeedback()
        {
            var memberId = User.FindFirst("Userid")?.Value; // جلب Userid من الجلسة

            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister"); // إعادة توجيه لتسجيل الدخول إذا لم يكن المستخدم مسجلًا
            }

            ViewBag.Userid = memberId; // إرسال Userid إلى الـ View

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback([Bind("Testimonialid,Content")] Testimonial testimonial)
        {
            var memberId = User.FindFirst("Userid")?.Value;

            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            testimonial.Userid = int.Parse(memberId); // ربط الـ Userid بالعضو الحالي

            if (ModelState.IsValid)
            {
                _context.Add(testimonial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "You have successfully Add Your Testimonial";
                return RedirectToAction(nameof(MemberDashboard));
            }
            
            return View(testimonial);
        }


        // GET: Member/Edit
        public async Task<IActionResult> EditMember()
        {
            var memberId = User.FindFirst("Userid")?.Value;
            ViewBag.memberId = memberId;

            if (memberId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            var member = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Userid == decimal.Parse(memberId));

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(User updatedUser)
        {

            try
            {

                var existingUser = await _context.Users
                       .Include(u => u.Profile)
                       .FirstOrDefaultAsync(u => u.Userid == updatedUser.Userid);


                existingUser.Email = updatedUser.Email;
                existingUser.Password = updatedUser.Password;
                existingUser.Roleid = 3;

                if (existingUser.Profile != null)
                {

                    existingUser.Profile.Fname = updatedUser.Profile.Fname;
                    existingUser.Profile.Lname = updatedUser.Profile.Lname;
                    existingUser.Profile.Gender = updatedUser.Profile.Gender;

                    if (updatedUser.Profile.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" + updatedUser.Profile.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await updatedUser.Profile.ImageFile.CopyToAsync(fileStream);
                        }

                        existingUser.Profile.Imagepath = fileName;
                    }

                }

              
                _context.Update(existingUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MemberDashboard));
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }



        }

        public IActionResult SubscriptionMember()
        {
            // الحصول على ID العضو
            var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MemberId");
            if (memberIdClaim == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

            if (!decimal.TryParse(memberIdClaim.Value, out decimal memberId))
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }
            var memberName = _context.Profiles
                .Where(m => m.Profileid == memberId)
                .Select(m => m.Fname) // Assuming the column for the member's name is 'Name'
                .FirstOrDefault();
            // مسار الفاتورة
            string invoiceFolderPath = Path.Combine("wwwroot", "invoices", memberId.ToString());
            string invoiceFilePath = Directory.Exists(invoiceFolderPath)
                ? Directory.GetFiles(invoiceFolderPath).FirstOrDefault()
                : null;

            // جلب الاشتراك الخاص بالعضو
            var subscription = _context.Subscriptions
                .Where(s => s.Memberid == memberId)
                .Select(s => new
                {
                    s.Plan.Planname,
                    s.Plan.Description,
                    s.Plan.Price,
                    s.Fromdate,
                    s.Todate,
                    InvoicePath = invoiceFilePath != null ? invoiceFilePath.Replace("wwwroot", "") : null
                })
                .FirstOrDefault();

            if (subscription == null)
            {
                ViewBag.Message = "You are not subscribed to any plans.";
                return View();
            }

            return View(subscription);
        }



    }
}
