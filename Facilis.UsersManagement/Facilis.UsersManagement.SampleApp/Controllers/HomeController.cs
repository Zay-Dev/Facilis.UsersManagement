using Microsoft.AspNetCore.Mvc;

namespace Facilis.UsersManagement.SampleApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}