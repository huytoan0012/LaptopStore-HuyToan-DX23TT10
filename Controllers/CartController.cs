using Microsoft.AspNetCore.Mvc;
using LaptopStore

.Services;
using LaptopStore

.Models;

namespace LaptopStore

.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        public IActionResult Index()
        {
            var cartItems = _cartService.GetCartItems();
            return View(cartItems);
        }

        // POST: /Cart/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            _cartService.AddToCart(productId, quantity);
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            return RedirectToAction("Index");
        }

        // POST: /Cart/RemoveFromCart
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            _cartService.RemoveFromCart(productId);
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        // GET: /Cart/ClearCart
        public IActionResult ClearCart()
        {
            _cartService.ClearCart();
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }

        // GET: /Cart/GetCartCount (API để hiển thị số lượng trên header)
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var count = _cartService.GetCartCount();
            return Json(new { count });
        }
    }
}