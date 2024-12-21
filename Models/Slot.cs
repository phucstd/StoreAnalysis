namespace StoreAnalysis.Models
{
    public class Slot
    {
        public int SlotID { get; set; } // Primary Key
        public string Name { get; set; } // Slot Name, e.g., A1, A2
        public bool IsEmpty { get; set; } // Slot empty status
        public DateTime LastRefillDate { get; set; } // Last refill date

        
    }

}
