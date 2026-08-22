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

        // GET: /Products
        public async Task<IActionResult> Index(int? brandId, string brandIds, int? minPrice, int? maxPrice, string searchString, string ram, string gpuChip, string screenSize, string storage, string cpu)
        {
            var products = _context.Products
                .Include(p => p.Brand)
                .Where(p => p.IsActive);

            // Lọc theo thương hiệu (1 brand - từ trang chủ)
            if (brandId.HasValue && brandId.Value > 0)
            {
                products = products.Where(p => p.BrandId == brandId.Value);
                ViewBag.SelectedBrand = await _context.Brands.FindAsync(brandId.Value);
            }

            // Lọc theo nhiều brand (từ checkbox trong sidebar)
            if (!string.IsNullOrEmpty(brandIds))
            {
                var brandIdList = brandIds.Split(',').Select(int.Parse).ToList();
                products = products.Where(p => brandIdList.Contains(p.BrandId));
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue && minPrice.Value > 0)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue && maxPrice.Value > 0)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            if (!string.IsNullOrWhiteSpace(ram))
            {
                products = products.Where(p => p.Specs != null && p.Specs.Contains(ram));
            }

            if (!string.IsNullOrWhiteSpace(gpuChip))
            {
                products = products.Where(p => p.Specs != null && p.Specs.Contains(gpuChip));
            }

            if (!string.IsNullOrWhiteSpace(screenSize))
            {
                products = products.Where(p => p.Specs != null && p.Specs.Contains(screenSize));
            }

            if (!string.IsNullOrWhiteSpace(storage))
            {
                products = products.Where(p => p.Specs != null && p.Specs.Contains(storage));
            }

            if (!string.IsNullOrWhiteSpace(cpu))
            {
                products = products.Where(p => p.Specs != null && p.Specs.Contains(cpu));
            }

            var productList = await products
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;

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

            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}