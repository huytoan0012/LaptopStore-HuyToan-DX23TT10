using LaptopStore

.Models;

namespace LaptopStore

.Services
{
    public interface ICartService
    {
        List<CartItem> GetCartItems();
        void AddToCart(int productId, int quantity);
        void UpdateQuantity(int productId, int quantity);
        void RemoveFromCart(int productId);
        void ClearCart();
        int GetCartCount();
        decimal GetCartTotal();
    }
}