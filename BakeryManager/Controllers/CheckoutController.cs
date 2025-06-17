using BakeryManager.Areas.Admin.Repository;
using BakeryManager.Models;
//using BakeryManager.Models.PayPal;
using BakeryManager.Repository;
using BakeryManager.Services.Momo;
//using BakeryManager.Services.PayPal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BakeryManager.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private IMomoService _momoService;
        //private readonly IPayPalService _payPalService;
        // private readonly IVnPayService _vnPayService;

        private static readonly HttpClient client = new HttpClient();
        public CheckoutController(IEmailSender emailSender, DataContext context, IMomoService momoService) //, IVnPayService vnPayService, IPayPalService payPalService
        {
            _dataContext = context;
            _emailSender = emailSender;
            _momoService = momoService;
            //_payPalService = payPalService;
            //   _vnPayService = vnPayService;

        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Checkout(string OrderId)
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
                // Retrieve shipping price from cookie
                var shippingPriceCookie = Request.Cookies["ShippingPrice"];
                decimal shippingPrice = 0;

                if (shippingPriceCookie != null)
                {
                    var shippingPriceJson = shippingPriceCookie;
                    shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
                }
                else
                {
                    shippingPrice = 0; // Default value if cookie is not set
                }
                orderItem.ShippingCost = shippingPrice;
                //Nhận coupon code
                var CouponCode = Request.Cookies["CouponTitle"];
                orderItem.CouponCode = CouponCode;
                //Nhận payment method
                if (OrderId != null)
                {
                    orderItem.PaymentMethod = "Momo";
                }
                else
                {
                    orderItem.PaymentMethod = "COD"; // Default payment method
                }
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
                    //update product quantity
                    var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                    product.Quantity -= cart.Quantity;
                    product.SoldOut += cart.Quantity;
                    _dataContext.Update(product);
                    _dataContext.Add(orderdetail);
                    _dataContext.SaveChanges();

                }
                HttpContext.Session.Remove("Cart");
                //Send mail order when success
                var receiver = userEmail;
                var subject = "Đặt hàng thành công";
                var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé.";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                TempData["success"] = "Đơn hàng đã được tạo,vui lòng chờ duyệt đơn hàng nhé.";
                return RedirectToAction("History", "Account");
            }
            return View();
        }

        //[HttpGet]
        //public IActionResult PaymentCallbackVnpay()
        //{
        //    var response = _vnPayService.PaymentExecute(Request.Query);

        //    return Json(response);
        //}
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfo model)
        {
            var response = await _momoService.CreatePaymentAsync(model);
            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInfoModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;

            if (requestQuery["resultCode"] != "0") //giao dich ko thanh cong
            {
                var newMomoInsert = new MomoInfoModel
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    DatePaid = DateTime.Now
                };

                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();
                
            }
            else
            {
                TempData["success"] = "Đã hủy giao dịch Momo.";
                return RedirectToAction("Index", "Cart");
            }

            // Gọi phương thức checkout sau khi lưu thông tin thanh toán
            var checkoutResult = await Checkout(requestQuery["orderId"]);
            return View(response);
        }

        //PayPal
        //public async Task<IActionResult> CreatePaymentUrl(PaymentInformationModel model)
        //{
        //    var url = await _payPalService.CreatePaymentUrl(model, HttpContext);

        //    return Redirect(url);
        //}

        //public IActionResult PaymentCallback()
        //{
        //    var response = _payPalService.PaymentExecute(Request.Query);

        //    return Json(response);
        //}


    }
}
