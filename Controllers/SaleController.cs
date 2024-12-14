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
            // Lấy ngày hiện tại và 6 ngày trước đó
            var startDate = DateTime.Today.AddDays(-6);
            var endDate = DateTime.Today;

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
                .Where(x => x.SaleDate.Date >= startDate && x.SaleDate.Date <= endDate) // Lọc theo ngày
                .GroupBy(x => x.SaleDate.Date) // Nhóm theo từng ngày
                .Select(g => new
                {
                    Day = g.Key,
                    Revenue = g.Sum(x => x.Price) // Tổng doanh thu trong ngày
                })
                .OrderBy(x => x.Day) // Sắp xếp theo ngày
                .ToList();

            // Chuyển đổi dữ liệu thành ViewModel
            var model = new RevenueChartViewModel
            {
                Labels = data.Select(x => x.Day.ToString("d MMM")).ToArray(), // Hiển thị ngày
                Data = data.Select(x => x.Revenue).ToArray() // Doanh thu theo ngày
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

        public ActionResult HourlySalesChart()
        {
            // Lấy ngày hiện tại và giờ hiện tại
            var today = DateTime.Today;
            var currentHour = DateTime.Now.Hour;

            // Lọc dữ liệu trong ngày hiện tại
            var salesToday = _context.Sales
                .Where(s => s.SaleDate.Date == today) // Chỉ lấy giao dịch của ngày hôm nay
                .GroupBy(s => s.SaleDate.Hour) // Nhóm theo giờ
                .Select(g => new
                {
                    Hour = g.Key,
                    Quantity = g.Count() // Tổng số lượng bán được trong giờ đó
                })
                .ToDictionary(g => g.Hour, g => g.Quantity); // Lưu vào Dictionary để tra cứu nhanh

            // Tạo danh sách từ 0h đến giờ hiện tại
            var data = Enumerable.Range(0, currentHour + 1)
                .Select(hour => new
                {
                    Hour = hour,
                    Quantity = salesToday.ContainsKey(hour) ? salesToday[hour] : 0 // Gán 0 nếu không có giao dịch
                })
                .ToList();

            // Tạo ViewModel
            var model = new HourlySalesChartViewModel
            {
                Hours = data.Select(x => $"{x.Hour}:00").ToArray(), // Chuyển giờ thành định dạng "0:00"
                Quantities = data.Select(x => x.Quantity).ToArray() // Số lượng bán theo giờ
            };

            return Json(model);
        }

    }

}
