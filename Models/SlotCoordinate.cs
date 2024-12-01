namespace StoreAnalysis.Models
{
    public class SlotCoordinate
    {
        public string Name { get; set; }
        public (int Min, int Max) XRange { get; set; }
        public (int Min, int Max) YRange { get; set; }
    }
}
