using BakeryManager.Models;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BakeryManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product")]
    [Authorize(Roles = "Admin, Staff")]
    public class ProductController : Controller
    {


        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnviroment = webHostEnvironment;
        }

        [Route("Index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(c => c.Category).ToListAsync());
        }

        [Route("CreateProductQuantity")]
        [HttpGet]
        public async Task<IActionResult> CreateProductQuantity(long Id)
        {
            var productbyquantity = await _dataContext.ProductQuantities
                .Where(pq => pq.ProductId == Id)
                .OrderByDescending(pq => pq.Date)
                .ToListAsync();

            ViewBag.ProductByQuantity = productbyquantity;
            ViewBag.ProductId = Id;

            return View();
        }

        [Route("UpdateMoreQuantity")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMoreQuantity(ProductQuantityModel productQuantityModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("CreateProductQuantity", "Product", new { Id = productQuantityModel.ProductId });
            }

            var product = await _dataContext.Products.FindAsync(productQuantityModel.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            // Cập nhật số lượng tổng
            product.Quantity = (product.Quantity ?? 0) + productQuantityModel.Quantity;

            // Lưu lịch sử thêm số lượng
            productQuantityModel.Date = DateTime.Now;

            _dataContext.Products.Update(product);
            _dataContext.ProductQuantities.Add(productQuantityModel);

            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Thêm số lượng sản phẩm thành công";
            return RedirectToAction("CreateProductQuantity", "Product", new { Id = productQuantityModel.ProductId });
        }


        [Route("Create")]

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
           
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");
                var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã có trong database");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");

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
                return BadRequest(errorMessage);
            }
            return View(product);
        }


        [Route("Edit")]

        public async Task<IActionResult> Edit(long Id)
        {
            ProductModel product = await _dataContext.Products.FindAsync(Id);
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
           

            return View(product);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            var existed_product = _dataContext.Products.Find(product.Id); //tìm sp theo id product
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-");

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = imageName;
                }


                // Update other product properties
                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryId = product.CategoryId;
               
                // ... other properties
                _dataContext.Update(existed_product);
                await _dataContext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");

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
                return BadRequest(errorMessage);
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long Id)
        {
            var product = await _dataContext.Products.FindAsync(Id);
            if (product == null) return NotFound();

            // Xóa ảnh nếu cần
            if (!string.Equals(product.Image, "noname.jpg"))
            {
                var uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                var oldFilePath = Path.Combine(uploadsDir, product.Image);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // Xóa dữ liệu liên quan trong ProductQuantities
            var productQuantities = _dataContext.ProductQuantities.Where(pq => pq.ProductId == product.Id);
            _dataContext.ProductQuantities.RemoveRange(productQuantities);

            // Xóa dữ liệu liên quan trong OrderDetails (phòng lỗi FK)
            var relatedOrderDetails = _dataContext.OrderDetails.Where(od => od.ProductId == product.Id);
            _dataContext.OrderDetails.RemoveRange(relatedOrderDetails);

            // Xóa dữ liệu liên quan trong Wishlist và Compare (nếu có)
            var relatedWishlist = _dataContext.Wishlists.Where(w => w.ProductId == product.Id);
            _dataContext.Wishlists.RemoveRange(relatedWishlist);

            var relatedCompare = _dataContext.Compares.Where(c => c.ProductId == product.Id);
            _dataContext.Compares.RemoveRange(relatedCompare);

            // Xóa sản phẩm
            _dataContext.Products.Remove(product);

            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }


    }
}
