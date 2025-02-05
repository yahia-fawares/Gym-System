using GymSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymSystem2.Controllers
{
    public class TrainerController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public TrainerController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public async Task< IActionResult> TrainerDashboard()
        {

            var TrainerId = User.FindFirst("Userid")?.Value;

            HttpContext.Session.SetString("Roleid", TrainerId);

            if (TrainerId == null)
            {
                return RedirectToAction("Login", "LoginAndResgister");
            }
            var trainer = await _context.Users
                .Include(u => u.Profile) // تضمين بيانات الملف الشخصي
                .Include(u => u.Subscriptions) // تضمين بيانات الاشتراكات
                .FirstOrDefaultAsync(u => u.Userid == decimal.Parse(TrainerId));

            if (trainer == null)
            {
                return NotFound();
            }

             
            return View(trainer);
     
        }

        public async Task<IActionResult> EditTrainer()
        {
           
            var trainerId = User.FindFirst("Userid")?.Value;
            ViewBag.trainerId = trainerId;
 
            if (trainerId == null)
            {
                return RedirectToAction("Login", "LoginAndRegister");
            }

             
            var trainer = await _context.Users
                .Include(u => u.Profile) 
                .FirstOrDefaultAsync(u => u.Userid == decimal.Parse(trainerId));
 
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(User updatedTrainer)
        {
            try
            {
                 
                var existingTrainer = await _context.Users
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.Userid == updatedTrainer.Userid);


                // تحديث بيانات المدرب
                existingTrainer.Email = updatedTrainer.Email;
                existingTrainer.Password = updatedTrainer.Password;
                existingTrainer.Roleid = 2;


                if (existingTrainer == null)
                {
                    return NotFound(); // إذا لم يتم العثور على المدرب
                }


                // تحديث بيانات الـ Profile إذا كانت موجودة
                if (existingTrainer.Profile != null)
                {
                    existingTrainer.Profile.Fname = updatedTrainer.Profile.Fname;
                    existingTrainer.Profile.Lname = updatedTrainer.Profile.Lname;
                    existingTrainer.Profile.ImageFile = updatedTrainer.Profile.ImageFile;
                    existingTrainer.Profile.Gender = updatedTrainer.Profile.Gender;

                    // تحديث صورة الملف الشخصي إذا كانت مرفقة
                    if (updatedTrainer.Profile.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" + updatedTrainer.Profile.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                        // حفظ الصورة في المسار المحدد
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await updatedTrainer.Profile.ImageFile.CopyToAsync(fileStream);
                        }

                        existingTrainer.Profile.Imagepath = fileName;
                    }
                }

                // تحديث الكيان في قاعدة البيانات
                _context.Update(existingTrainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TrainerDashboard)); 
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }



        public async Task<IActionResult> Subscriptions()
        {
            // استرجاع TrainerId من الـ Claims
            var currentTrainerId = User.FindFirst("TrainerId")?.Value;

            if (currentTrainerId == null)
            {
                return Unauthorized(); // إذا لم يكن هناك TrainerId في الـ Claims، يجب أن يتم إرجاع رد غير مصرح به
            }

            var modelContext = await _context.Subscriptions
                .Include(s => s.Member) // تضمين بيانات الـ Member
                .ThenInclude(m => m.Profile) // تضمين الـ Profile الخاص بـ Member
                .Include(s => s.Plan) // تضمين بيانات الخطة
                .Where(s => s.Plan.Trainerid.ToString() == currentTrainerId) // تحقق من أن الخطة تخص المدرب الحالي
                .ToListAsync();

            return View(modelContext);
        }


        public async Task<IActionResult> SubscriptionsDetails(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            // استرجاع TrainerId من الـ Claims
            var currentTrainerId = User.FindFirst("TrainerId")?.Value;

            if (currentTrainerId == null)
            {
                return Unauthorized(); // إذا لم يكن هناك TrainerId في الـ Claims، يجب أن يتم إرجاع رد غير مصرح به
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member) // تضمين بيانات الـ Member
                .ThenInclude(m => m.Profile) // تضمين الـ Profile الخاص بـ Member
                .Include(s => s.Plan) // تضمين بيانات الخطة
                .FirstOrDefaultAsync(m => m.Subscriptionid == id && m.Plan.Trainerid.ToString() == currentTrainerId); // تحقق من أن الخطة تخص المدرب الحالي

            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }


        public IActionResult Search()
        {
            // استرجاع TrainerId من الـ Claims
            var currentTrainerId = User.FindFirst("TrainerId")?.Value;

            if (currentTrainerId == null)
            {
                return Unauthorized(); // إذا لم يكن هناك TrainerId في الـ Claims، يجب أن يتم إرجاع رد غير مصرح به
            }

            // جلب البيانات من جدول الاشتراكات وربطها بالجدول المرتبط (مثل Member و Plan)
            var result = _context.Subscriptions
                .Include(x => x.Plan)
                .Include(x => x.Member)
                .ThenInclude(m => m.Profile)
                .Where(x => x.Plan.Trainerid.ToString() == currentTrainerId) // إضافة التحقق من TrainerId
                .ToList();

            return View(result);
        }

        [HttpPost]
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            // استرجاع TrainerId من الـ Claims
            var currentTrainerId = User.FindFirst("TrainerId")?.Value;

            if (currentTrainerId == null)
            {
                return Unauthorized(); // إذا لم يكن هناك TrainerId في الـ Claims، يجب أن يتم إرجاع رد غير مصرح به
            }

            var result = _context.Subscriptions
                .Include(x => x.Plan)
                .Include(x => x.Member)
                .ThenInclude(m => m.Profile)
                .Where(x => x.Plan.Trainerid.ToString() == currentTrainerId) // إضافة التحقق من TrainerId
                .ToList();

            // فلترة البيانات بناءً على التاريخ إذا تم تمرير تواريخ
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


        // GET: Plans
        public async Task<IActionResult> Plans()
        {
             var currentTrainerId = User.FindFirst("TrainerId")?.Value;

             var plans = await _context.Plans
                .Where(p => p.Trainerid.ToString() == currentTrainerId)
                .ToListAsync();

            if (plans == null || !plans.Any())
            {
                ViewBag.NoPlans = true;
            }

            return View(plans);
        }

        // GET: Plans/Create
        public IActionResult CreatePlans()
        {
            return View();
        }

        // POST: Plans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlans([Bind("Planid,Planname,Description,Price")] Plan plan)
        {
            // استرداد TrainerId من الجلسة
            var currentTrainerId = User.FindFirst("TrainerId")?.Value;

            if (ModelState.IsValid && currentTrainerId != null)
            {
                plan.Trainerid = decimal.Parse(currentTrainerId);  
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Plans));
            }

            return View(plan);
        }
        // GET: Plans/Edit/5
        public async Task<IActionResult> EditPlans(decimal? id)
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
        public async Task<IActionResult> EditPlans(decimal id, [Bind("Planid,Planname,Description,Price,Trainerid")] Plan plan)
        {
            var currentTrainerId =HttpContext.Session.GetInt32("Trainerid");


            if (id != plan.Planid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    plan.Trainerid = currentTrainerId;
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
                return RedirectToAction("EditPlans");
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
        public async Task<IActionResult> DeletePlans(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .FirstOrDefaultAsync(m => m.Planid == id);

             var currentTrainerId = User.FindFirst("TrainerId")?.Value;
            if (plan == null || plan.Trainerid.ToString() != currentTrainerId)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlans(decimal id)
        {
            var plan = await _context.Plans.FindAsync(id);

             var currentTrainerId = User.FindFirst("TrainerId")?.Value;
            if (plan != null && plan.Trainerid.ToString() == currentTrainerId)
            {
                _context.Plans.Remove(plan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Plans));
        }

        // التحقق من وجود الخطة
        private bool PlanExists(decimal id)
        {
            return _context.Plans.Any(e => e.Planid == id);
        }



    }






}
