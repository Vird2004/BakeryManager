using BakeryManager.Models;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BakeryManager.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckoutController(DataContext context) 
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var ordercode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = ordercode;

                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.OrderDate = DateTime.Now;

                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();

                //tạo order detail
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach (var cart in cartItems)
                {
                    var orderdetail = new OrderDetails();
                    orderdetail.UserName = userEmail;
                    orderdetail.OrderCode = ordercode;
                    orderdetail.ProductId = cart.ProductId;
                    orderdetail.Price = cart.Price;
                    orderdetail.Quantity = cart.Quantity;

                    _dataContext.Add(orderdetail);
                    _dataContext.SaveChanges();

                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Đơn hàng đã được tạo,vui lòng chờ duyệt đơn hàng nhé.";
                return RedirectToAction("Index", "Cart");
            }
                return View();
        }
    }
}
