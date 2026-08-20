using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore.Data;
using LaptopStore.Models;

namespace LaptopStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Brands = await _context.Brands.ToListAsync();
            return View();
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
{
    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        string errorMsg = "Vui lòng kiểm tra lại dữ liệu: ";
        foreach (var error in errors)
        {
            errorMsg += error.ErrorMessage + "; ";
        }
        TempData["Error"] = errorMsg;

        ViewBag.Brands = await _context.Brands.ToListAsync();
        return View(product);
    }

    try
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            product.ImageUrl = $"/images/products/{fileName}";
        }

        product.CreatedDate = DateTime.Now;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        TempData["Success"] = "✅ Thêm sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        TempData["Error"] = $"❌ Lỗi hệ thống: {ex.Message}";
        ViewBag.Brands = await _context.Brands.ToListAsync();
        return View(product);
    }
}

public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var product = await _context.Products.FindAsync(id);
    if (product == null)
    {
        return NotFound();
    }

    ViewBag.Brands = await _context.Brands.ToListAsync();
    return View(product);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
{
    if (id != product.Id)
    {
        return NotFound();
    }

    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        string errorMsg = "Vui lòng kiểm tra lại dữ liệu: ";
        foreach (var error in errors)
        {
            errorMsg += error.ErrorMessage + "; ";
        }
        TempData["Error"] = errorMsg;
        ViewBag.Brands = await _context.Brands.ToListAsync();
        return View(product);
    }

    try
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.Specs = product.Specs;
        existingProduct.BrandId = product.BrandId;
        existingProduct.IsActive = product.IsActive;

        if (imageFile != null && imageFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                    existingProduct.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            existingProduct.ImageUrl = $"/images/products/{fileName}";
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "✅ Cập nhật sản phẩm thành công!";
        return RedirectToAction(nameof(Index));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"LỖI KHI CẬP NHẬT: {ex.Message}");
        TempData["Error"] = $"❌ Lỗi: {ex.Message}";
        ViewBag.Brands = await _context.Brands.ToListAsync();
        return View(product);
    }
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Xóa sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI KHI XÓA: {ex.Message}");
                TempData["Error"] = $"Không thể xóa sản phẩm: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                product.IsActive = !product.IsActive;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã {(product.IsActive ? "hiển thị" : "ẩn")} sản phẩm!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LỖI KHI THAY ĐỔI TRẠNG THÁI: {ex.Message}");
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}