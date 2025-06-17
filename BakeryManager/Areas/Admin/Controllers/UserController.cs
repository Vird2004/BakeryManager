using BakeryManager.Models;
using BakeryManager.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BakeryManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Fix: Changed UserManager<IdentityRole> to RoleManager<IdentityRole>
        private readonly DataContext _dataContext;

        public UserController(DataContext context, UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager) // Fix: Updated constructor parameter type
        {
            _dataContext = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await (from u in _dataContext.Users
                                        join ur in _dataContext.UserRoles on u.Id equals ur.UserId into ur_join
                                        from ur in ur_join.DefaultIfEmpty()
                                        join r in _dataContext.Roles on ur.RoleId equals r.Id into r_join
                                        from r in r_join.DefaultIfEmpty()
                                        select new
                                        {
                                            User = u,
                                            RoleName = r != null ? r.Name : "No Role"
                                        })
                       .ToListAsync();


            return View(usersWithRoles);
        }

        [HttpGet]
        [Route("Create")]
        public async Task<IActionResult> Create()
        {
            var roles = await _roleManager.Roles.ToListAsync(); // Fix: _roleManager now correctly references RoleManager<IdentityRole>
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(new AppUserModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id, AppUserModel user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Cập nhật thông tin cơ bản
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;

                // Cập nhật thông tin user
                var updateUserResult = await _userManager.UpdateAsync(existingUser);
                if (!updateUserResult.Succeeded)
                {
                    AddIdentityErrors(updateUserResult);
                    return View(existingUser);
                }

                // ✅ Xử lý Role đúng cách
                // Lấy Role hiện tại
                var currentRoles = await _userManager.GetRolesAsync(existingUser);

                // Xóa Role cũ
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        AddIdentityErrors(removeResult);
                        return View(existingUser);
                    }
                }

                // Thêm Role mới từ RoleId
                if (!string.IsNullOrEmpty(user.RoleId))
                {
                    // Tìm tên Role tương ứng từ Id
                    var role = await _roleManager.FindByIdAsync(user.RoleId);
                    if (role != null)
                    {
                        var addRoleResult = await _userManager.AddToRoleAsync(existingUser, role.Name);
                        if (!addRoleResult.Succeeded)
                        {
                            AddIdentityErrors(addRoleResult);
                            return View(existingUser);
                        }
                    }
                }

                TempData["success"] = "Cập nhật thành công!";
                return RedirectToAction("Index", "User");
            }

            // Trường hợp lỗi validation
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name", user.RoleId);
            TempData["error"] = "Model validation failed.";

            return View(existingUser);
        }

        private void AddIdentityErrors(IdentityResult identityResult)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user)
        {
            if(ModelState.IsValid)
            {
                var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash); //tạo user
                if (createUserResult.Succeeded)
                {
                    var createUser = await _userManager.FindByEmailAsync(user.Email); //tìm user dựa vào email
                    var userId = createUser.Id; // lấy user Id
                    var role = _roleManager.FindByIdAsync(user.RoleId); //lấy RoleId
                                                                        //gán quyền
                    var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in createUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    return RedirectToAction("Index", "User");
                }
                else
                {

                    foreach (var error in createUserResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(user);
                }
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
            var roles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View(user);
        }

        [HttpGet]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return View("Error");
            }
            TempData["success"] = "User đã được xóa thành công";
            return RedirectToAction("Index");
        }
    }
}
