namespace StoreAnalysis.Models
{
    public class Item
    {
        public int ItemID { get; set; } // Primary Key
        public string ItemName { get; set; } // Item Name
        public int Quantity { get; set; } // Quantity of items in the slot
        public float Price { get; set; } // Price of the item
        public DateTime AddedDate { get; set; } // When item was added

        // Foreign Key for Slot
        public int SlotID { get; set; }
        public Slot Slot { get; set; }
    }

}
