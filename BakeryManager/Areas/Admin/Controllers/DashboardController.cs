using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BakeryManager.Models;
using BakeryManager.Repository;
using System.Globalization;

namespace BakeryManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;

        public DashboardController(DataContext context)
        {
            _dataContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.CountProduct = _dataContext.Products.Count();
            ViewBag.CountOrder = _dataContext.Orders.Count();
            ViewBag.CountCategory = _dataContext.Categories.Count();
            ViewBag.CountUser = _dataContext.Users.Count();

            return View();
        }

        [HttpPost]
        [Route("GetChartData")]
        public IActionResult GetChartData()
        {
            var chartData = _dataContext.Orders
                .Join(_dataContext.OrderDetails,
                      o => o.OrderCode,
                      od => od.OrderCode,
                      (o, od) => new { o.OrderDate, od.Quantity, od.Price, od.ProductId })
                .Join(_dataContext.Products,
                      od => od.ProductId,
                      p => p.Id,
                      (od, p) => new
                      {
                          od.OrderDate,
                          Revenue = od.Quantity * od.Price,
                          Cost = od.Quantity * p.CapitalPrice,
                          Profit = (od.Quantity * od.Price) - (od.Quantity * p.CapitalPrice)
                      })
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    revenue = g.Sum(x => x.Revenue),
                    cost = g.Sum(x => x.Cost),
                    profit = g.Sum(x => x.Profit),
                    orders = g.Count()
                })
                .ToList();

            return Json(chartData);
        }


        [HttpPost]
        [Route("SubmitFilterDate")]
        public IActionResult SubmitFilterDate(string filterdate)
        {
            if (!DateTime.TryParse(filterdate, out DateTime parsedDate))
            {
                return BadRequest("Invalid date format.");
            }

            var chartData = _dataContext.Orders
                .Where(o => o.OrderDate.Date == parsedDate.Date)
                .Join(_dataContext.OrderDetails,
                      o => o.OrderCode,
                      od => od.OrderCode,
                      (o, od) => new { o.OrderDate, od.Quantity, od.Price, od.ProductId })
                .Join(_dataContext.Products,
                      od => od.ProductId,
                      p => p.Id,
                      (od, p) => new
                      {
                          od.OrderDate,
                          Revenue = od.Quantity * od.Price,
                          Cost = od.Quantity * p.CapitalPrice,
                          Profit = (od.Quantity * od.Price) - (od.Quantity * p.CapitalPrice)
                      })
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    revenue = g.Sum(x => x.Revenue),
                    cost = g.Sum(x => x.Cost),
                    profit = g.Sum(x => x.Profit),
                    orders = g.Count()
                })
                .ToList();

            return Json(chartData);
        }


        [HttpPost]
        [Route("SelectFilterDate")]
        public IActionResult SelectFilterDate(string filterdate)
        {
            DateTime today = DateTime.Today;
            DateTime start, end;

            switch (filterdate)
            {
                case "last_month":
                    start = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                    end = new DateTime(today.Year, today.Month, 1).AddDays(-1);
                    break;

                case "this_month":
                    start = new DateTime(today.Year, today.Month, 1);
                    end = today;
                    break;

                case "all_year":
                    start = new DateTime(today.Year, 1, 1);
                    end = today;
                    break;

                default:
                    return BadRequest("Invalid filter.");
            }

            var chartData = _dataContext.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate <= end)
                .Join(_dataContext.OrderDetails,
                      o => o.OrderCode,
                      od => od.OrderCode,
                      (o, od) => new { o.OrderDate, od.Quantity, od.Price, od.ProductId })
                .Join(_dataContext.Products,
                      od => od.ProductId,
                      p => p.Id,
                      (od, p) => new
                      {
                          od.OrderDate,
                          Revenue = od.Quantity * od.Price,
                          Cost = od.Quantity * p.CapitalPrice,
                          Profit = (od.Quantity * od.Price) - (od.Quantity * p.CapitalPrice)
                      })
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    revenue = g.Sum(x => x.Revenue),
                    cost = g.Sum(x => x.Cost),
                    profit = g.Sum(x => x.Profit),
                    orders = g.Count()
                })
                .ToList();

            return Json(chartData);
        }

    }
}
