using BakeryManager.Models;
using BakeryManager.Models.ViewModels;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BakeryManager.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;

        public ProductController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            var products = await _dataContext.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();

            ViewBag.Keyword = searchTerm;

            return View(products);
        }

        public async Task<IActionResult> Details(long Id)
        {
            var product = await _dataContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _dataContext.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != Id)
                .Take(4)
                .ToListAsync();

            var ratings = await _dataContext.Ratings
                .Where(r => r.ProductId == Id)
                .ToListAsync();

            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = product
            };

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.Ratings = ratings;

            return View(viewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            if (ModelState.IsValid)
            {

                var ratingEntity = new RatingModel
                {
                    ProductId = rating.ProductId,
                    Name = rating.Name,
                    Email = rating.Email,
                    Comment = rating.Comment,
                    Stars = rating.Stars

                };

                _dataContext.Ratings.Add(ratingEntity);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm đánh giá thành công";

                return Redirect(Request.Headers["Referer"]);
            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);

                return RedirectToAction("Detail", new { id = rating.ProductId });
            }

            return Redirect(Request.Headers["Referer"]);
        }
    }
}
