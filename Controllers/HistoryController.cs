using Microsoft.AspNetCore.Mvc;
using StoreAnalysis.Data;

namespace StoreAnalysis.Controllers
{
    public class HistoryController : Controller
    {
        private readonly StoreAnalysisContext _context;

        public HistoryController(StoreAnalysisContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.AnalysisImages.ToList());
        }
        public IActionResult ShowImage(int id, string type)
        {
            string path = "";
            if(type == "processed")
            {
                path = "processed/" + _context.AnalysisImages.FirstOrDefault(_ => _.Id.Equals(id))?.AnalyzedImagePath;
            }
            else
            {
                path = "uploads/" + _context.AnalysisImages.FirstOrDefault(_ => _.Id.Equals(id))?.CameraImagePath;
            }
            return View(model:path);
        }
    }
}
