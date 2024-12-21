namespace StoreAnalysis.Models
{
    public class Item
    {
        public int Id { get; set; }
        // Foreign Key for Slot
        public string ItemId { get; set; }
        public int SlotID { get; set; }
        public Slot Slot { get; set; }
    }

}
