using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LaptopStore

.Models;

namespace LaptopStore

.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.EnsureCreatedAsync();

            await context.Database.ExecuteSqlRawAsync("""
                IF COL_LENGTH('dbo.Orders', 'PaymentMethod') IS NULL
                    ALTER TABLE [dbo].[Orders] ADD [PaymentMethod] nvarchar(50) NULL;
                """);

            if (!context.Brands.Any())
            {
                var brands = new Brand[]
                {
                    new Brand { Name = "Dell", Description = "Thương hiệu máy tính hàng đầu thế giới", LogoUrl = "/images/brands/dell.png" },
                    new Brand { Name = "HP", Description = "Thương hiệu máy tính nổi tiếng của Mỹ", LogoUrl = "/images/brands/hp.png" },
                    new Brand { Name = "Lenovo", Description = "Thương hiệu máy tính số 1 Trung Quốc", LogoUrl = "/images/brands/lenovo.png" },
                    new Brand { Name = "Asus", Description = "Thương hiệu máy tính và linh kiện Đài Loan", LogoUrl = "/images/brands/asus.png" },
                    new Brand { Name = "Acer", Description = "Thương hiệu máy tính giá rẻ phổ biến", LogoUrl = "/images/brands/acer.png" },
                    new Brand { Name = "MSI", Description = "Thương hiệu máy tính chuyên gaming", LogoUrl = "/images/brands/msi.png" }
                };
                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }

            if (!context.Products.Any())
            {
                var dell = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Dell");
                var hp = await context.Brands.FirstOrDefaultAsync(b => b.Name == "HP");
                var lenovo = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Lenovo");
                var asus = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Asus");
                var acer = await context.Brands.FirstOrDefaultAsync(b => b.Name == "Acer");
                var msi = await context.Brands.FirstOrDefaultAsync(b => b.Name == "MSI");

                var products = new Product[]
                {
                    new Product
                    {
                        Name = "Dell XPS 13 Plus",
                        Price = 32990000,
                        Description = "Laptop cao cấp với thiết kế siêu mỏng, màn hình OLED 13.4 inch, vi xử lý Intel Core i7-1360P",
                        StockQuantity = 15,
                        ImageUrl = "/images/products/dell-xps-13.jpg",
                        Specs = "CPU: Intel Core i7-1360P | RAM: 16GB LPDDR5 | SSD: 512GB | Màn hình: 13.4\" OLED 3.5K",
                        BrandId = dell.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Dell Inspiron 16",
                        Price = 18990000,
                        Description = "Laptop văn phòng màn hình lớn, hiệu năng ổn định, pin lâu dài",
                        StockQuantity = 25,
                        ImageUrl = "/images/products/dell-inspiron-16.jpg",
                        Specs = "CPU: Intel Core i5-13420H | RAM: 8GB DDR4 | SSD: 512GB | Màn hình: 16\" 1920x1200",
                        BrandId = dell.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "HP Spectre x360",
                        Price = 38990000,
                        Description = "Laptop 2 trong 1 gập linh hoạt, màn hình cảm ứng, thiết kế sang trọng",
                        StockQuantity = 10,
                        ImageUrl = "/images/products/hp-spectre-x360.jpg",
                        Specs = "CPU: Intel Core i7-1355U | RAM: 16GB LPDDR5 | SSD: 1TB | Màn hình: 13.5\" OLED 3K",
                        BrandId = hp.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "HP EliteBook 840 G10",
                        Price = 28990000,
                        Description = "Laptop doanh nghiệp cao cấp, bảo mật vân tay, chịu được va đập",
                        StockQuantity = 18,
                        ImageUrl = "/images/products/hp-elitebook.jpg",
                        Specs = "CPU: Intel Core i7-1355U | RAM: 16GB DDR5 | SSD: 512GB | Màn hình: 14\" 1920x1200",
                        BrandId = hp.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Lenovo ThinkPad X1 Carbon Gen 11",
                        Price = 45990000,
                        Description = "Laptop doanh nhân đẳng cấp, siêu nhẹ 1.12kg, pin 15 giờ",
                        StockQuantity = 8,
                        ImageUrl = "/images/products/thinkpad-x1.jpg",
                        Specs = "CPU: Intel Core i7-1365U | RAM: 16GB LPDDR5 | SSD: 1TB | Màn hình: 14\" 4K",
                        BrandId = lenovo.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Lenovo Legion 5 Pro",
                        Price = 33990000,
                        Description = "Laptop gaming mạnh mẽ với card đồ họa RTX 4060, màn hình 16 inch 240Hz",
                        StockQuantity = 12,
                        ImageUrl = "/images/products/legion-5-pro.jpg",
                        Specs = "CPU: AMD Ryzen 7 7840H | RAM: 16GB DDR5 | SSD: 1TB | VGA: RTX 4060 8GB",
                        BrandId = lenovo.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Asus ROG Zephyrus G14",
                        Price = 29990000,
                        Description = "Laptop gaming nhỏ gọn, hiệu năng cao, thiết kế LED Matrix độc đáo",
                        StockQuantity = 20,
                        ImageUrl = "/images/products/zephyrus-g14.jpg",
                        Specs = "CPU: AMD Ryzen 9 7940HS | RAM: 16GB DDR5 | SSD: 1TB | VGA: RTX 4060 8GB",
                        BrandId = asus.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Asus Zenbook 14 OLED",
                        Price = 21990000,
                        Description = "Laptop siêu mỏng nhẹ, màn hình OLED 14 inch, pin 10 giờ",
                        StockQuantity = 22,
                        ImageUrl = "/images/products/zenbook-14.jpg",
                        Specs = "CPU: Intel Core i7-1360P | RAM: 16GB LPDDR5 | SSD: 512GB | Màn hình: 14\" OLED 2.8K",
                        BrandId = asus.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Acer Swift 3",
                        Price = 15990000,
                        Description = "Laptop tầm trung hiệu năng tốt, thiết kế trẻ trung, pin lâu dài",
                        StockQuantity = 30,
                        ImageUrl = "/images/products/acer-swift-3.jpg",
                        Specs = "CPU: Intel Core i5-1335U | RAM: 8GB LPDDR5 | SSD: 512GB | Màn hình: 14\" 1920x1080",
                        BrandId = acer.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new Product
                    {
                        Name = "MSI Stealth 14 Studio",
                        Price = 35990000,
                        Description = "Laptop gaming mỏng nhất thế giới, hiệu năng cực mạnh",
                        StockQuantity = 10,
                        ImageUrl = "/images/products/msi-stealth-14.jpg",
                        Specs = "CPU: Intel Core i7-13620H | RAM: 16GB DDR5 | SSD: 1TB | VGA: RTX 4060 8GB",
                        BrandId = msi.Id,
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                };
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            const string adminEmail = "admin@LaptopStore.com";
            const string adminPassword = "Admin@123";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}