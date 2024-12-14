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
    }

    public class HourlySalesChartViewModel
    {
        public string[] Hours { get; set; } // Giờ (00:00, 01:00, ...)
        public int[] Quantities { get; set; }  // Số lượng bán mỗi giờ
    }
}
