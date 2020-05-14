using MacosApp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MacosApp.Web.Controllers
{
    public class accountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}