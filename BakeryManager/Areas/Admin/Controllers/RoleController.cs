using BakeryManager.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BakeryManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Role")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _dataContext = context;
            _roleManager = roleManager;
        }


        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _dataContext.Roles.OrderByDescending(p => p.Id).ToListAsync());
        }


    }
}
