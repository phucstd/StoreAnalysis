namespace StoreAnalysis.Models
{
    public class ItemStorage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemName { get; set; } // Item Name
        public float Price { get; set; } // Price of the item
        public DateTime AddedDate { get; set; } // When item was added
    }
}
