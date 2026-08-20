using LaptopStore.Data;
using LaptopStore.Models;
using LaptopStore.Services;
using LaptopStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LaptopStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartItems();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            SetCartViewData(cart);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = _cartService.GetCartItems();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                SetCartViewData(cart);
                return View(model);
            }

            try
            {
                var order = new Order
                {
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    RecipientName = model.RecipientName,
                    PhoneNumber = model.PhoneNumber,
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                    OrderDetails = cart.Select(item => new OrderDetail
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList()
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _cartService.ClearCart();
                TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: #{order.Id}";
                return RedirectToAction(nameof(OrderSuccess), new { id = order.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi đặt hàng: {ex.Message}";
                SetCartViewData(cart);
                return View(model);
            }
        }

        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _context.Orders
                .Include(item => item.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .FirstOrDefaultAsync(item => item.Id == id);

            return order == null ? NotFound() : View(order);
        }

        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(order => order.UserId == userId)
                .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        private void SetCartViewData(List<CartItem> cart)
        {
            ViewBag.Cart = cart;
        }
    }
}