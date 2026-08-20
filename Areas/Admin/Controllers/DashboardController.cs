using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore.Data;
using LaptopStore.Models;
using LaptopStore.ViewModels;

namespace LaptopStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Index()
        {
             Console.WriteLine("=== DASHBOARD INDEX CALLED ===");
            var today = DateTime.Today;


            // ===== 1. TỔNG QUAN =====
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed" || o.Status == "Shipped")
                .SumAsync(o => o.TotalAmount);
            var todayOrders = await _context.Orders
                .Where(o => o.OrderDate >= today)
                .CountAsync();
            var todayRevenue = await _context.Orders
                .Where(o => o.OrderDate >= today && (o.Status == "Completed" || o.Status == "Shipped"))
                .SumAsync(o => o.TotalAmount);
            var totalProducts = await _context.Products.CountAsync();
            var totalUsers = await _context.Users.CountAsync();

            // ===== 2. DOANH THU THEO THÁNG (12 tháng) =====
            var monthlyRevenues = new List<MonthlyRevenue>();
            // Lấy 12 tháng GẦN NHẤT (bao gồm tháng hiện tại)
for (int i = 11; i >= 0; i--)
{
    var month = DateTime.Now.AddMonths(-i);
    var startDate = new DateTime(month.Year, month.Month, 1);
    var endDate = startDate.AddMonths(1);

    var revenue = await _context.Orders
        .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
        .Where(o => o.Status == "Completed" || o.Status == "Shipped")
        .SumAsync(o => o.TotalAmount);

    // Log để kiểm tra (xem Terminal)
    Console.WriteLine($"Tháng: {startDate:MM/yyyy}, Doanh thu: {revenue}");

    monthlyRevenues.Add(new MonthlyRevenue
    {
        Month = month.ToString("MMM yyyy"),
        Revenue = revenue,
        OrderCount = 0
    });
}

            // ===== 3. TOP SẢN PHẨM BÁN CHẠY =====
            var topProducts = await _context.OrderDetails
                .Include(od => od.Product)
                .GroupBy(od => od.ProductId)
                .Select(g => new TopProduct
                {
                    ProductName = g.First().Product!.Name,
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(5)
                .ToListAsync();

            // ===== 4. THỐNG KÊ TRẠNG THÁI ĐƠN HÀNG (SỬA LỖI NULL) =====
            var orderStatusCounts = await _context.Orders
                .GroupBy(o => o.Status ?? "Chưa cập nhật")
                .Select(g => new OrderStatusCount
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Nếu không có dữ liệu, thêm mặc định để hiển thị
            if (!orderStatusCounts.Any())
            {
                orderStatusCounts.Add(new OrderStatusCount { Status = "Chưa có đơn hàng", Count = 1 });
            }

            // ===== 5. ĐƠN HÀNG GẦN ĐÂY =====
            var recentOrders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .Select(o => new RecentOrder
                {
                    Id = o.Id,
                    RecipientName = o.RecipientName,
                    TotalAmount = o.TotalAmount,
                    OrderDate = o.OrderDate,
                    Status = o.Status ?? "Chưa cập nhật"
                })
                .ToListAsync();

            var viewModel = new StatisticsViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TodayOrders = todayOrders,
                TodayRevenue = todayRevenue,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers,
                MonthlyRevenues = monthlyRevenues,
                TopProducts = topProducts,
                OrderStatusCounts = orderStatusCounts,
                RecentOrders = recentOrders
            };

            return View(viewModel);
        }

        // GET: /Admin/Dashboard/ThongKe
        public async Task<IActionResult> ThongKe()
        {
            return await Index();
        }
    }
}