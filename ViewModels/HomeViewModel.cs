using LaptopStore

.Models;

namespace LaptopStore

.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> LatestProducts { get; set; } = new List<Product>();
        public List<Product> FeaturedProducts { get; set; } = new List<Product>();
        public List<Brand> Brands { get; set; } = new List<Brand>();
    }
}