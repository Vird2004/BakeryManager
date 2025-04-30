using BakeryManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BakeryManager.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
    }
    
    }

