namespace StoreAnalysis.Models
{
    public class ItemStorage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; } // Price of the item
        public DateTime LastUpdatedDate { get; set; }
        public int Amount { get; set; }
    }
}
