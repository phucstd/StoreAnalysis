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
        // GET: Statistics
        public IActionResult Statistics()
        {
            var mostSoldItem = _context.Sales
                .GroupBy(s => s.ItemID)
                .Select(g => new
                {
                    ItemID = g.Key,
                    TotalSold = g.Sum(s => s.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .FirstOrDefault();

            var item = _context.Items.FirstOrDefault(i => i.ItemID == mostSoldItem.ItemID);

            ViewBag.MostSoldItem = item?.ItemName;
            return View();
        }
    }

}
