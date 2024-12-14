using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAnalysis.Data;
using StoreAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult RevenueChart()
        {
            var data = _context.Sales
                        .Join(
                            _context.ItemsStorage,  // Bảng cần join
                            sale => sale.ItemId,    // Khóa ngoại trong bảng Sales
                            item => item.Id,        // Khóa chính trong bảng ItemsStorage
                            (sale, item) => new     // Kết quả join
                            {
                                SaleDate = sale.SaleDate,
                                Price = item.Price
                            }
                        )
                        .GroupBy(x => x.SaleDate.Month) // Nhóm theo tháng
                        .Select(g => new
                        {
                            Month = g.Key,
                            Revenue = g.Sum(x => x.Price) // Tổng doanh thu của tháng
                        })
                        .OrderBy(x => x.Month)
                        .ToList();


            var model = new RevenueChartViewModel
            {
                Labels = data.Select(x => $"Month {x.Month}").ToArray(),
                Data = data.Select(x => x.Revenue).ToArray()
            };

            return Json(model);
        }

        // 2. Biểu đồ sản phẩm bán chạy
        public ActionResult ProductSalesChart()
        {
            var data = _context.Sales
                .GroupBy(s => s.ItemId)
                .Select(g => new
                {
                    ItemName = _context.ItemsStorage.FirstOrDefault(i => i.Id == g.Key).ItemName,
                    Quantity = g.Count()
                })
                .OrderByDescending(x => x.Quantity)
                .ToList();

            var model = new ProductSalesChartViewModel
            {
                ItemNames = data.Select(x => x.ItemName).ToArray(),
                Quantities = data.Select(x => x.Quantity).ToArray()
            };

            return Json(model);
        }

        // 3. Biểu đồ bán hàng theo giờ
        public ActionResult HourlySalesChart()
        {
            var data = _context.Sales
                .GroupBy(s => s.SaleDate.Hour) // Nhóm theo giờ
                .Select(g => new
                {
                    Hour = g.Key,
                    Quantity = g.Count()
                })
                .OrderBy(x => x.Hour)
                .ToList();

            var model = new HourlySalesChartViewModel
            {
                Hours = data.Select(x => $"{x.Hour}:00").ToArray(),
                Quantities = data.Select(x => x.Quantity).ToArray()
            };

            return Json(model);
        }
    }

}
