using Microsoft.AspNetCore.Identity;

namespace LaptopStore

.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsAdmin { get; set; }
        public bool IsLockedOut => LockoutEnabled && LockoutEnd > DateTimeOffset.UtcNow;
        public string Status => IsLockedOut ? "🔒 Đã khóa" : "✅ Hoạt động";
        public string StatusColor => IsLockedOut ? "danger" : "success";
    }
}