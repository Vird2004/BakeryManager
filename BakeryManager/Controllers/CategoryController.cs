using BakeryManager.Models;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BakeryManager.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            CategoryModel category = _dataContext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();
            
            if(category == null)
            {
                return RedirectToAction("Index");
            }

            var productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            return View(await productsByCategory.OrderByDescending(c=>c.Id).ToListAsync());
        }
    }
}
