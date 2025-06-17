using BakeryManager.Areas.Admin.Repository;
using BakeryManager.Hubs;
using BakeryManager.Models;
using BakeryManager.Models.MoMo;
using BakeryManager.Repository;
using BakeryManager.Services.Gemini;
using BakeryManager.Services.Momo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//Connect MomoAPI
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

//PayPal 
//builder.Services.Configure<PayPalOptionModel>(builder.Configuration.GetSection("PayPal"));
//builder.Services.AddScoped<IPayPalService, PayPalService>();
//ConnectionDb GetConnectionString("ConnectedDb")
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

//Add Email Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

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
//Configuration login google account
builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});

builder.Services.AddRazorPages();
// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;

   
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:7044")
                .AllowAnyHeader()
               
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });

});

//Add SignalR
builder.Services.AddSignalR();

// ✅ Đăng ký HttpClient
builder.Services.AddHttpClient();

// ✅ Đăng ký GeminiService
builder.Services.AddScoped<IGeminiService, GeminiService>();

var app = builder.Build();
//page 404 Error
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

//session
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.UseHttpsRedirection();




app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();
// UseCors must be called before MapHub.
app.UseCors("CorsPolicy");
//Set Signal Hub

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

// UseCors must be called before MapHub.
app.MapHub<ChatHub>("/Realtime/Index");

app.Run();
