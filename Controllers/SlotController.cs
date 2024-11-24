using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAnalysis.Data;
using StoreAnalysis.Models;

namespace StoreAnalysis.Controllers
{
    public class SlotController : Controller
    {
        private readonly StoreAnalysisContext _context;

        public SlotController(StoreAnalysisContext context)
        {
            _context = context;
        }

        // GET: Display all slots
        public IActionResult Index()
        {
            var slots = _context.Slots.Include(s => s.Items).ToList();
            return View(slots);
        }

        // POST: Refill a slot
        [HttpPost]
        public IActionResult Refill(int slotId, string itemName, int quantity, float price)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return NotFound();

            // Update slot
            slot.IsEmpty = false;
            slot.LastRefillDate = DateTime.Now;

            // Add item
            var item = new Item
            {
                SlotID = slotId,
                ItemName = itemName,
                Quantity = quantity,
                Price = price,
                AddedDate = DateTime.Now
            };
            _context.Items.Add(item);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

      
        public void EmptySlot(int slotId)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return;

            // Set slot as empty
            slot.IsEmpty = true;

            // Remove all items in the slot
            var items = _context.Items.Where(i => i.SlotID == slotId);
            _context.Items.RemoveRange(items);
            _context.SaveChanges();
        }
    }

}
