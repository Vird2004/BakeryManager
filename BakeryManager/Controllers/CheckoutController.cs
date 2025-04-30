using Microsoft.AspNetCore.Mvc;

namespace BakeryManager.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
