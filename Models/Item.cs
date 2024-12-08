namespace StoreAnalysis.Models
{
    public class Item
    {
        public string Id { get; set; }
        // Foreign Key for Slot
        public int SlotID { get; set; }
        public Slot Slot { get; set; }
    }

}
