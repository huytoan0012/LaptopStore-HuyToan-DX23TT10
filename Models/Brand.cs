using System.ComponentModel.DataAnnotations;

namespace LaptopStore

.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên hãng không được để trống")]
        [StringLength(100, ErrorMessage = "Tên hãng tối đa 100 ký tự")]
        public string Name { get; set; }

        public string? LogoUrl { get; set; } // Đường dẫn logo

        public string? Description { get; set; } // Mô tả hãng

        // Quan hệ 1-n: Một hãng có nhiều sản phẩm
        public virtual ICollection<Product>? Products { get; set; }
    }
}