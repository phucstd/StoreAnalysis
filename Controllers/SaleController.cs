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
            // Lấy ngày hôm nay và tuần trước
            var today = DateTime.Today;
            var startOfThisWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfLastWeek = startOfThisWeek.AddDays(-7);

            // Doanh thu tuần này
            var thisWeekRevenue = _context.Sales
                .Where(s => s.SaleDate.Date >= startOfThisWeek && s.SaleDate.Date <= today)
                .Sum(s => s.ItemStorage.Price);

            // Doanh thu tuần trước
            var lastWeekRevenue = _context.Sales
                .Where(s => s.SaleDate.Date >= startOfLastWeek && s.SaleDate.Date < startOfThisWeek)
                .Sum(s => s.ItemStorage.Price);

            // Tổng số lượng giao dịch tuần này
            var thisWeekTotalSales = _context.Sales
                .Count(s => s.SaleDate.Date >= startOfThisWeek && s.SaleDate.Date <= today);

            // Tổng số lượng giao dịch tuần trước
            var lastWeekTotalSales = _context.Sales
                .Count(s => s.SaleDate.Date >= startOfLastWeek && s.SaleDate.Date < startOfThisWeek);

            // Doanh thu gần đây
            var recentSales = _context.Sales
                .Include(s => s.ItemStorage)
                .OrderByDescending(s => s.SaleDate)
                .Take(10)
                .ToList();

            // Biểu đồ sản phẩm bán chạy
            var productSalesData = _context.Sales
                .GroupBy(s => s.ItemStorage)
                .Select(g => new
                {
                    ItemName = g.Key.ItemName,
                    Quantity = g.Count(),
                    Price = g.Key.Price,
                    TotalPrice = g.Sum(s => g.Key.Price)
                })
                .OrderByDescending(x => x.Quantity)
                .ToList();

            var productSalesChartViewModel = new ProductSalesChartViewModel
            {
                ItemNames = productSalesData.Select(x => x.ItemName).ToArray(),
                Quantities = productSalesData.Select(x => x.Quantity).ToArray(),
                Price = productSalesData.Select(x => x.Price).ToArray(),
                TotalPrice = productSalesData.Select(x => x.TotalPrice).ToArray()
            };

            // Tính toán tăng trưởng
            var growingRevenuePercentCompareLastWeek = lastWeekRevenue > 0 ? ((thisWeekRevenue - lastWeekRevenue) / lastWeekRevenue) * 100
                : 100;

            var growingTotalSalesPercentCompareLastWeek = lastWeekTotalSales > 0
                ? ((thisWeekTotalSales - lastWeekTotalSales) / (float)lastWeekTotalSales) * 100
                : 100;

            // Tạo SaleViewModel
            var saleViewModel = new SaleViewModel
            {
                TotalSales = thisWeekTotalSales,
                GrowingTotalSalesPercentCompareLastWeek = growingTotalSalesPercentCompareLastWeek,
                Revenue = thisWeekRevenue,
                GrowingRevenuePercentCompareLastWeek = growingRevenuePercentCompareLastWeek,
                productSalesChartViewModel = productSalesChartViewModel,
                RecentSales = recentSales,
                notifications = _context.Notifications.OrderByDescending(s => s.CreatedDate).Take(5).ToList()

            };

            return View(saleViewModel);
        }
        public ActionResult HourlySalesChart()
        {
            var today = DateTime.Today;

            // Get sales data grouped by hour
            var hourlySalesData = _context.Sales
                .Where(s => s.SaleDate.Date == today) // Filter sales for today
                .GroupBy(s => s.SaleDate.Hour)
                .Select(g => new
                {
                    Hour = g.Key,
                    Quantity = g.Count(),
                    Revenue = g.Sum(s => s.ItemStorage.Price)
                })
                .ToDictionary(g => g.Hour);

            // Generate hourly chart model with DateTime for ApexCharts
            var hourlySalesAndRevenueChartModel = new HourlySalesAndRevenueChartViewModel
            {
                Hours = Enumerable.Range(0, DateTime.Now.Hour + 1)
                    .Select(h => today.AddHours(h)) // Create DateTime for each hour
                    .ToArray(),
                Quantities = Enumerable.Range(0, DateTime.Now.Hour + 1)
                    .Select(h => hourlySalesData.ContainsKey(h) ? hourlySalesData[h].Quantity : 0)
                    .ToArray(),
                Revenue = Enumerable.Range(0, DateTime.Now.Hour + 1)
                    .Select(h => hourlySalesData.ContainsKey(h) ? hourlySalesData[h].Revenue : 0)
                    .ToArray()
            };

            return Json(hourlySalesAndRevenueChartModel);
        }

    }

}
