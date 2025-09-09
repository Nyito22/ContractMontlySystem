using Microsoft.AspNetCore.Mvc;

namespace ContractMontlySystem.Controllers
{
    public class Employee : Controller
    {
        public IActionResult Lecture()
        {
            return View();
        }

        public IActionResult pc()
        {
            return View();
        }
        public IActionResult am()
        {
            return View();
        }
        public IActionResult ViewClaim()
        {
            return View();
        }
    }
}
