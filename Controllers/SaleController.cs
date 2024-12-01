using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAnalysis.Data;

namespace StoreAnalysis.Controllers
{
    public class SaleController : Controller
    {
        private readonly StoreAnalysisContext _context;

        public SaleController(StoreAnalysisContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }

}
