using BakeryManager.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BakeryManager.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context) { 
            _context.Database.Migrate();

            if (!_context.Products.Any())
            {
                CategoryModel cake = new CategoryModel
                {
                    Name = "Cupcakes",
                    Description = "Mini cakes in cups, topped with frosting and sprinkles. Perfect for parties and gifts.",
                    Slug = "cupcakes",
                    Status = "1"
                };
                CategoryModel cookie = new CategoryModel
                {
                    Name = "Cookies",
                    Slug = "cookies",
                    Description = "Crunchy, chewy or soft-baked cookies in various flavors like chocolate chip, almond, and matcha.",
                    Status = "1"
                };
                

                _context.Products.AddRange(
                   new ProductModel { Category = cake, Name = "Chocolate Cupcake", Description = "Rich chocolate cupcake with creamy frosting.", Price = 25000, Slug = "chocolate-cupcake", Image = "1.jpg", Status = "1" },

                 new ProductModel { Category = cookie, Name = "Chocolate Chip Cookie", Description = "Classic chocolate chip cookie, soft and chewy.", Price = 15000, Slug = "chocolate-chip-cookie", Image = "2.jpg", Status = "1" }
                );
                _context.SaveChanges();
            }
        }
    }
}
