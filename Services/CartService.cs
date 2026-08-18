using LaptopStore

.Models;
using Microsoft.EntityFrameworkCore;
using LaptopStore

.Data;

namespace LaptopStore

.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private List<CartItem> GetCartFromSession()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return new List<CartItem>();

            var cartJson = session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }

            return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return;

            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            session.SetString("Cart", cartJson);
        }

        public List<CartItem> GetCartItems()
        {
            return GetCartFromSession();
        }

        public void AddToCart(int productId, int quantity)
        {
            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = _context.Products.Include(p => p.Brand).FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = productId,
                        ProductName = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = quantity
                    });
                }
            }

            SaveCartToSession(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Remove(item);
                }
                else
                {
                    item.Quantity = quantity;
                }
                SaveCartToSession(cart);
            }
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);
            if (item != null)
            {
                cart.Remove(item);
                SaveCartToSession(cart);
            }
        }

        public void ClearCart()
        {
            SaveCartToSession(new List<CartItem>());
        }

        public int GetCartCount()
        {
            var cart = GetCartFromSession();
            return cart.Sum(c => c.Quantity);
        }

        public decimal GetCartTotal()
        {
            var cart = GetCartFromSession();
            return cart.Sum(c => c.Total);
        }
    }
}