namespace StoreAnalysis.Models
{
    public class Detection
    {
        public int[] box { get; set; } // [x, y, width, height]
        public float score { get; set; }
        public string lable { get; set; }
    }
}
