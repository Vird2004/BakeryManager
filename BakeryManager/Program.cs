using BakeryManager.Models;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//ConnectionDb GetConnectionString("ConnectedDb")
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});



//Khai bao Identity
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

   
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

//page 404 Error
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//session
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Areas",
    pattern: "{areas:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "category",
    pattern: "/category/{Slug?}",
    defaults : new {controller="Category", action = "Index"});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedingData(context);

app.Run();
