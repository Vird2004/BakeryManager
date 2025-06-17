using Microsoft.AspNetCore.Mvc;

namespace BakeryManager.Controllers
{
    public class RealtimeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
