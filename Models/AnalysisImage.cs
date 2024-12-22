namespace StoreAnalysis.Models
{
    public class AnalysisImage
    {
        public int Id { get; set; }
        public string CameraImagePath { get; set; }

        public string AnalyzedImagePath { get; set; }

        public DateTime Date { get; set; }
    }
}
