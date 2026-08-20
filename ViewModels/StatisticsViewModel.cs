namespace LaptopStore.ViewModels
{
    public class StatisticsViewModel
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodayOrders { get; set; }

        public List<MonthlyRevenue> MonthlyRevenues { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<OrderStatusCount> OrderStatusCounts { get; set; } = new();
        public List<RecentOrder> RecentOrders { get; set; } = new();
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProduct
    {
        public string? ProductName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class OrderStatusCount
    {
        public string? Status { get; set; }
        public int Count { get; set; }
    }

    public class RecentOrder
    {
        public int Id { get; set; }
        public string? RecipientName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; }
    }
}