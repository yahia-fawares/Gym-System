using Microsoft.AspNetCore.Mvc;
using GymSystem2.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace GymSystem2.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Profile,Fname,Lname,Email,Password,Roleid.Cardid")] User user, int roleid)
        {
            if (ModelState.IsValid)
            {
                // Handle Image upload
                if (user.Profile.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath; // Assuming this is where you want to save images
                    string fileName = Guid.NewGuid().ToString() + "_" + user.Profile.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    // Upload file to server
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await user.Profile.ImageFile.CopyToAsync(fileStream);
                    }

                    
                    user.Profile.Imagepath = fileName;
                }

                 
                _context.Add(user.Profile);   
                await _context.SaveChangesAsync(); 

                 
                user.Profileid = user.Profile.Profileid;  

                 
                user.Roleid = 3;
                user.Cardid = 111;

                // Add User
                _context.Add(user);  
                await _context.SaveChangesAsync();  

                // Redirect to Login page
                return RedirectToAction("Login");
            }

            return View(user);   
        }




        // Display the login form
        public IActionResult Login()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            
            if (ModelState.IsValid)
            {
               
                var user = await _context.Users
                    .Include(u => u.Profile)  
                    .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

                if (user != null)
                {


                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Profile.Fname), // اسم المستخدم
                        new Claim("UserId", user.Userid.ToString()),    // معرّف المستخدم
                        new Claim("RoleId", user.Roleid.ToString()),
                        new Claim("MemberId", user.Userid.ToString()),
                        new Claim("TrainerId", user.Userid.ToString()),
                        new Claim(ClaimTypes.Email, user.Email)  
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                    HttpContext.Session.SetInt32("Roleid",(int)user.Roleid);
                    HttpContext.Session.SetInt32("Trainerid", (int)user.Userid);



                    switch (user.Roleid)
                    {


                        case 1://Admin
                            HttpContext.Session.SetString("AdminName", user.Profile.Fname);
                            return RedirectToAction("Index", "Admin");

                        case 2://Trainer 
                            HttpContext.Session.SetString("TrainerName", user.Profile.Fname);
                            return RedirectToAction("TrainerDashboard", "Trainer");

                        case 3://Member
                            HttpContext.Session.SetString("MemberName", user.Profile.Fname);
                            return RedirectToAction("Index", "Home");

                    }



                }
                else
                {
                     
                   ViewBag.Error =   "Invalid login attempt.";
                }
            }

            
            return View();
        }

        [HttpPost]
        public IActionResult LogoutSession()
        {
            HttpContext.Session.Clear(); // تفريغ جميع بيانات الـ Session
            return RedirectToAction("Login", "LoginAndRegister"); // توجيه المستخدم إلى صفحة تسجيل الدخول
        }


    }
}
