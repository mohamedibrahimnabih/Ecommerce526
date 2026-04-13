using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.Identity.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
