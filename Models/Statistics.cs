namespace StoreAnalysis.Models
{
    public class RevenueChartViewModel
    {
        public string[] Labels { get; set; } // Ví dụ: Tháng, Ngày
        public float[] Data { get; set; } // Doanh thu từng mục
    }

    public class ProductSalesChartViewModel
    {
        public string[] ItemNames { get; set; } // Tên sản phẩm
        public int[] Quantities { get; set; }  // Số lượng bán

        public float[] Price; 
        public float[] TotalPrice { get; set; }
    }

    public class HourlySalesAndRevenueChartViewModel
    {
        public DateTime[] Hours { get; set; } // Time with DateTime format
        public int[] Quantities { get; set; }  // Số lượng bán mỗi giờ

        public float[] Revenue { get; set; }
    }

    public class SaleViewModel()
    {
        public int TotalSales { get; set; }
        public float GrowingTotalSalesPercentCompareLastWeek { get; set; }
        public float Revenue { get; set; }

        public float GrowingRevenuePercentCompareLastWeek {  get; set; }

        public List<Sale> RecentSales { get; set; }

        public ProductSalesChartViewModel productSalesChartViewModel { get; set; }

        public List<Notification> notifications { get; set; }

    }
}
