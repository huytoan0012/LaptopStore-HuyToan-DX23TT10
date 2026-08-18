using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore

.ViewModels;

namespace LaptopStore

.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isAdmin = roles.Contains("Admin");

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd,
                    Roles = roles.ToList(),
                    IsAdmin = isAdmin
                });
            }

            return View(userViewModels);
        }

        // GET: /Admin/User/Details/5
        public async Task<IActionResult> Details(string id)
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

            var roles = await _userManager.GetRolesAsync(user);
            var isAdmin = roles.Contains("Admin");

            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = roles.ToList(),
                IsAdmin = isAdmin
            };

            return View(model);
        }

        // POST: /Admin/User/Lock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(string id)
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

            // Không cho khóa tài khoản Admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Không thể khóa tài khoản Admin!";
                return RedirectToAction(nameof(Index));
            }

            // Khóa tài khoản đến năm 2099
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(75);
            user.LockoutEnabled = true;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Đã khóa tài khoản {user.Email}!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/User/Unlock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(string id)
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

            user.LockoutEnd = null;
            user.LockoutEnabled = false;
            await _userManager.UpdateAsync(user);

            TempData["Success"] = $"Đã mở khóa tài khoản {user.Email}!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/User/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // Không cho xóa tài khoản Admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["Error"] = "Không thể xóa tài khoản Admin!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = $"Đã xóa tài khoản {user.Email}!";
            }
            else
            {
                TempData["Error"] = "Xóa tài khoản thất bại!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/User/MakeAdmin
        [HttpGet]
        public async Task<IActionResult> MakeAdmin(string id)
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

            // Kiểm tra role Admin đã tồn tại chưa
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Xóa role User nếu có (không bắt buộc)
            if (await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
            }

            // Thêm role Admin
            await _userManager.AddToRoleAsync(user, "Admin");

            TempData["Success"] = $"Đã gán quyền Admin cho {user.Email}!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/User/RemoveAdmin
        [HttpGet]
        public async Task<IActionResult> RemoveAdmin(string id)
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

            // Không cho xóa quyền Admin của chính mình
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id == id)
            {
                TempData["Error"] = "Bạn không thể xóa quyền Admin của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.RemoveFromRoleAsync(user, "Admin");

            // Thêm lại role User
            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }
            await _userManager.AddToRoleAsync(user, "User");

            TempData["Success"] = $"Đã xóa quyền Admin của {user.Email}!";
            return RedirectToAction(nameof(Index));
        }
    }
}