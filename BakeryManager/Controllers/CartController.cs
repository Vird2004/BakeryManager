using BakeryManager.Models;
using BakeryManager.Models.ViewModels;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BakeryManager.Controllers
{
    public class CartController : Controller
    {
        public readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
    }
}
