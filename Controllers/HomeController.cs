using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore

.Data;
using LaptopStore

.Models;
using LaptopStore

.ViewModels;  // ← Thêm dòng này
using System.Diagnostics;

namespace LaptopStore

.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                LatestProducts = await _context.Products
                    .Include(p => p.Brand)
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(8)
                    .ToListAsync(),

                FeaturedProducts = await _context.Products
                    .Include(p => p.Brand)
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.Price)
                    .Take(4)
                    .ToListAsync(),

                Brands = await _context.Brands.ToListAsync()
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}