using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaptopStore

.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra xem User đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            
            return View();
        }
    }
}