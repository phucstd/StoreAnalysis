namespace StoreAnalysis.Models
{
    public class Sale
    {
        public int SaleID { get; set; } // Primary Key
        public int ItemID { get; set; } // Foreign Key for Item
        public DateTime SaleDate { get; set; } // Date of sale
    }

}
