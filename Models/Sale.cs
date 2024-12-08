using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreAnalysis.Models
{
    public class Sale
    {
        public int? SaleId { get; set; } // Primary Key
        public string ItemId { get; set; } // Foreign Key for Item
        public DateTime SaleDate { get; set; } // Date of sale
    }

}
