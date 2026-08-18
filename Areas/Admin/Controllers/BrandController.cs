using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LaptopStore.Data;
using LaptopStore.Models;

namespace LaptopStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Brand
        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands.OrderBy(b => b.Name).ToListAsync();
            return View(brands);
        }

        // GET: /Admin/Brand/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm hãng sản xuất thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // GET: /Admin/Brand/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return NotFound();

            return View(brand);
        }

        // POST: /Admin/Brand/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand brand)
        {
            if (id != brand.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật hãng sản xuất thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Brands.Any(e => e.Id == brand.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        // POST: /Admin/Brand/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null) return NotFound();

            // Kiểm tra xem hãng có sản phẩm không
            if (brand.Products != null && brand.Products.Any())
            {
                TempData["Error"] = $"Không thể xóa hãng '{brand.Name}' vì đang có {brand.Products.Count} sản phẩm thuộc hãng này!";
                return RedirectToAction(nameof(Index));
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xóa hãng sản xuất thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}