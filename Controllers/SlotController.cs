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
            var slots = _context.Slots
                .Include(s => s.Items) // Include related items
                .ToList();

            return View(slots);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Refill(RefillItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Debugging code to see what is invalid
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View(model);  // If the form data is not valid, return to the same view.
            }


            // Fetch the slot by SlotID from the database.
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == model.SlotID);
            if (slot == null)
            {
                return NotFound(); // If the slot is not found, return a NotFound result.
            }

            // Update slot details
            slot.IsEmpty = false;
            slot.LastRefillDate = DateTime.Now;

            var newItemStorage = new ItemStorage()
            {
                Id = model.Id,
                AddedDate = DateTime.Now,
                ItemName = model.ItemName,
                Price = model.Price,
            };

            // Add new item to the slot
            var item = new Item
            {
                SlotID = model.SlotID,
                Id = model.Id
            };

            // Add the item to the database
            _context.Items.Add(item);
            _context.SaveChanges(); // Save changes to persist the new item.

            TempData["Message"] = $"Item '{item.Id}' added to Slot {slot.Name}.";
            return RedirectToAction("Index"); // Redirect back to the Index page.
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EmptySlot(int slotId)
        {
            var slot = _context.Slots.Include(s => s.Items).FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return NotFound();
            try
            {
                // Log sales before deleting items
                foreach (var item in slot.Items)
                {
                    var sale = new Sale
                    {
                        ItemId = item.Id,
                        SaleDate = DateTime.Now
                    };
                    _context.Sales.Add(sale);
                }
                slot.Items.Clear();
                var itemsList = _context.Items.Where(_ => _.SlotID == slotId);
                // Save sales first
                _context.SaveChanges();
                // Remove items and mark slot empty
                _context.Items.RemoveRange(itemsList);
                slot.IsEmpty = true;
                _context.SaveChanges();
                TempData["Message"] = $"Slot {slot.Name} has been cleared and items have been logged.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["Message"] = "An error occurred while clearing the slot.";
            }

            return RedirectToAction("Index");
        }


        // GET: Display Refill Form
        public IActionResult Refill(int slotId)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return NotFound();

            var model = new RefillItemViewModel
            {
                SlotID = slot.SlotID
            };

            return View(model);
        }

        

    }

}
