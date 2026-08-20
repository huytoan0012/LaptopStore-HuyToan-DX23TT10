using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore.Data;
using LaptopStore.Models;

namespace LaptopStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Products?brandId=1
        public async Task<IActionResult> Index(int? brandId, string searchString)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Lọc theo thương hiệu
            if (brandId.HasValue && brandId.Value > 0)
            {
                products = products.Where(p => p.BrandId == brandId.Value);
                ViewBag.SelectedBrand = await _context.Brands.FindAsync(brandId.Value);
            }

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            var productList = await products.OrderByDescending(p => p.CreatedDate).ToListAsync();
            ViewBag.Brands = await _context.Brands.ToListAsync();

            return View(productList);
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy thêm 4 sản phẩm cùng hãng để gợi ý
            var relatedProducts = await _context.Products
                .Include(p => p.Brand)
                .Where(p => p.BrandId == product.BrandId && p.Id != product.Id && p.IsActive)
                .Take(4)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}