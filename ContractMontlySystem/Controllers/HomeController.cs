using System.Diagnostics;
using ContractMontlySystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            sql_connection lets_connect = new sql_connection();
            lets_connect.createUserTable();
            return RedirectToAction("home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register user)
        {
            sql_connection lets_connect = new sql_connection();

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            else
            {
               lets_connect.Store_into_Table(user.FullName,user.PhoneNumber, user.Email, user.Password,user.Role);
                return RedirectToAction("Login");
            }

           
        }
        public IActionResult home()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // HttpPost action to handle the login form submission
        [HttpPost]
        public IActionResult Login(Login user)
        {
            sql_connection conect = new sql_connection();

            if (ModelState.IsValid)
            {
                
                bool isAuthenticated = conect.LogInUser(user.Email, user.Password, user.Role);

                if (isAuthenticated)
                {
                    
                    switch (user.Role)  // Switch based on user role
                    {
                        case "Lecture":
                            return RedirectToAction("Lecture", "Employee"); 
                        case "ProgrammeCoordinator":
                            return RedirectToAction("pc", "Employee");

                        case "AcademicManager":
                            return RedirectToAction("am", "Employee");
                        default:
                            return RedirectToAction("home", "Home"); 
                    }
                }
                else
                {
                    
                    ModelState.AddModelError("", "Invalid login credentials.");
                    return View(user); 
                }
            }

            // If the model is not valid, return to the login page with validation errors
            return View(user);
        }



        public IActionResult Privacy()
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
