using GymSystem2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Numerics;
using System.IO; // للتعامل مع الملفات
using iText.Kernel.Pdf; // مكتبة PDF الأساسية
using iText.Layout; // للتعامل مع تخطيط المستند
using iText.Layout.Element;
using System.Security.Claims;


namespace GymSystem2.Controllers
{
    public class payament : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public payament(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }


        public class PdfGenerator
        {
            // دالة لإنشاء فاتورة بصيغة PDF
            public static void GenerateInvoicePDF(string filePath)
            {
                // إنشاء كاتب PDF وتحديد المسار
                using (var writer = new PdfWriter(filePath))
                {
                    // إنشاء مستند PDF باستخدام الكاتب
                    using (var pdf = new PdfDocument(writer))
                    {
                        // إنشاء مستند تخطيط
                        var document = new Document(pdf);

                        // إضافة عنوان إلى المستند
                        document.Add(new Paragraph("Invoice").SetFontSize(18));

                        // إضافة فقرة شكر
                        document.Add(new Paragraph("Thank you for your purchase."));

                        // إنهاء المستند
                    }
                }
            }
        }
        private string GenerateInvoicePDF(decimal memberId, string memberName, Plan plan)
        {
            string folderPath = Path.Combine("wwwroot", "invoices", memberId.ToString());
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string invoicePath = Path.Combine(folderPath, $"Invoice_{DateTime.Now.Ticks}.pdf");

            using (var stream = new FileStream(invoicePath, FileMode.Create))
            {
                using (var writer = new PdfWriter(stream))
                {
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    document.Add(new Paragraph("Invoice").SetFontSize(20));
                    document.Add(new Paragraph($"Member Name: {memberName}"));  // عرض اسم العضو
                    document.Add(new Paragraph($"Plan Name: {plan.Planname}"));
                    document.Add(new Paragraph($"Price: {plan.Price}"));
                    document.Add(new Paragraph($"Date: {DateTime.Now}"));

                    document.Close();
                }
            }

            return invoicePath; // إرجاع المسار الكامل للفاتورة
        }




        private void SendInvoiceEmail(string email, string invoicePath)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("yahiafares190@gmail.com"), // بريدك الإلكتروني كمصدر
                    Subject = "Subscription Invoice",
                    Body = "Thank you for your payment. Please find your invoice attached.",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);  

                // إضافة الفاتورة كملف مرفق
                mailMessage.Attachments.Add(new Attachment(invoicePath));

                // إعدادات خادم SMTP
                using (var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("yahiafares190@gmail.com", "wpgr qrqi pqln bntl"), // استخدم كلمة المرور الخاصة بالتطبيق هنا
                    EnableSsl = true
                })
                {
                    smtpClient.Send(mailMessage);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
            
        }











        // GET - عرض صفحة الدفع
        public IActionResult Checkout(int planId)
        {
            var plan = _context.Plans.FirstOrDefault(p => p.Planid == planId);
            if (plan == null)
            {
                return NotFound();
            }

            // تحقق من أن العضو قد اشترك بالفعل في نفس الخطة
            var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MemberId");
            if (memberIdClaim != null)
            {
                decimal memberId;
                if (decimal.TryParse(memberIdClaim.Value, out memberId))
                {
                    // تحقق إذا كان العضو قد اشترك مسبقًا في نفس الخطة
                    var existingSubscription = _context.Subscriptions.FirstOrDefault(s => s.Memberid == memberId && s.Planid == planId);
                    if (existingSubscription != null)
                    {
                        TempData["ErrorMessage"] = "You are already subscribed to this plan.";
                        return View(plan);
                    }
                }
            }

            return View(plan);
        }

        // POST - معالجة الاشتراك وخصم من البطاقة
        [HttpPost]
        public IActionResult Checkout(int planId, string cardNumber, string cardPassword)
        {
             if (!long.TryParse(cardNumber, out long cardNumberLong))
            {
                ModelState.AddModelError("", "Card number is invalid.");
                return View(_context.Plans.FirstOrDefault(p => p.Planid == planId));
            }

             if (!long.TryParse(cardPassword, out long cardPasswordLong))
            {
                ModelState.AddModelError("", "Card password is invalid.");
                return View(_context.Plans.FirstOrDefault(p => p.Planid == planId));
            }

             var plan = _context.Plans.FirstOrDefault(p => p.Planid == planId);
            if (plan == null)
            {
                return NotFound();
            }

            // التحقق من صحة البطاقة
            var card = _context.Cards.FirstOrDefault(c => c.Cardid == cardNumberLong && c.Password == cardPasswordLong);
            if (card == null)
            {
                ModelState.AddModelError("", "Invalid card details. Please check your card number and password.");
                return View(plan);
            }

            if (card.Balance <= 0)
            {
                ModelState.AddModelError("", "Insufficient balance in the card.");
                return View(plan);
            }

            // جلب ID العضو من Claims
            var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == "MemberId");
            if (memberIdClaim == null)
            {
                ModelState.AddModelError("", "You must be logged in to subscribe.");
                return View(plan);
            }

            if (!decimal.TryParse(memberIdClaim.Value, out decimal memberId))
            {
                ModelState.AddModelError("", "Invalid member ID.");
                return View(plan);
            }

            // جلب بيانات العضو (بما في ذلك الاسم)
            var member = _context.Users.Include(u => u.Profile).FirstOrDefault(m => m.Userid == memberId);
            if (member == null)
            {
                ModelState.AddModelError("", "Member not found.");
                return View(plan);
            }

           
            if (member.Profile == null)
            {
                ModelState.AddModelError("", "Member profile is missing.");
                return View(plan);
            }

            var memberName = member.Profile.Fname + " " + member.Profile.Lname; 

            // التحقق من الاشتراك الحالي
            var existingSubscription = _context.Subscriptions.FirstOrDefault(s => s.Memberid == memberId);
            if (existingSubscription != null)
            {
                ModelState.AddModelError("", "You are already subscribed to another plan. Cancel your current plan before subscribing to a new one.");
                return View(plan);
            }

             card.Balance -= plan.Price;
            _context.SaveChanges();

            // إنشاء اشتراك جديد
            var subscription = new Subscription
            {
                Memberid = Convert.ToInt64(memberId),
                Planid = planId,
                Fromdate = DateTime.Now,
                Todate = DateTime.Now.AddMonths(5)
            };

             var invoicePath = GenerateInvoicePDF(memberId, memberName, plan);
            subscription.InvoicePath = invoicePath;

            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();

            // جلب البريد الإلكتروني للعضو
            var memberEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(memberEmail))
            {
                ModelState.AddModelError("", "Email is not available in your account.");
                return View(plan);
            }

             try
            {
                SendInvoiceEmail(memberEmail, invoicePath);  
            }
            catch (Exception ex)
            {
                TempData["WarningMessage"] = "Subscription successful, but we couldn't send the invoice to your email.";
            }

            TempData["SuccessMessage"] = "You have successfully subscribed! The invoice has been sent to your email.";
            return View(plan);
        }







    }
}
