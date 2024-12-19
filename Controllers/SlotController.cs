// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreAnalysis.Data;
using StoreAnalysis.Models;
using StoreAnalysis.Script;

namespace StoreAnalysis.Controllers
{
    public class SlotController : Controller
    {
        private readonly StoreAnalysisContext _context;
        private readonly TelegramService _telegramService;
        public SlotController(StoreAnalysisContext context, TelegramService telegramService)
        {
            _context = context;
            _telegramService = telegramService;
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
        public async Task<IActionResult> Refill(RefillItemViewModel model)
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

            var itemStorage = _context.ItemsStorage.FirstOrDefault(_ => _.Id.Equals(model.Id));
            if ( itemStorage == null)
            {
                TempData["Message"] = $"Item storage not found";
                return RedirectToAction("Refill", new { slotId = model.SlotID });
            }

            if (itemStorage.Amount <= 0)
            {
                TempData["Message"] = $"Item in storage is out of stock";
                return RedirectToAction("Refill", new { slotId = model.SlotID });
            }

            // Fetch the slot by SlotID from the database.
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == model.SlotID);
            if (slot == null)
            {
                TempData["Message"] = $"Slot not found";
                return RedirectToAction("Refill", new { slotId = model.SlotID }); // If the slot is not found, return a NotFound result.
            }

            // Update slot details
            slot.IsEmpty = false;
            slot.LastRefillDate = DateTime.Now;
            if(slot.Items == null)
            {
                slot.Items = new List<ItemStorage>();
            }
            slot.Items.Add(itemStorage);
            itemStorage.Amount--;
            // Add new item to the slot
            var item = new Item
            {
                SlotID = model.SlotID,
                Id = model.Id
            };

            // Add the item to the database
            _context.Items.Add(item);
            await _context.SaveChangesAsync(); // Save changes to persist the new item.
            var message = await SendMessage(new Notification($"{itemStorage.Name} had been filled into  {slot.Name}", "All", 1));
            TempData["Message"] = $"Item '{item.Id}' added to Slot {slot.Name}. \n{message}";
            TempData["Status"] = "Success";
            return RedirectToAction("Refill", new { slotId = model.SlotID });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmptySlot(int slotId)
        {
            await EmptySlotAndSendMessage(slotId);
            return RedirectToAction("Index");
        }


        // GET: Display Refill Form
        public IActionResult Refill(int slotId)
        {
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return NotFound();
            ViewBag.Slot = slot;
            ViewBag.AvailableItems = _context.ItemsStorage.Where(i => i.Amount > 0).ToList(); // Fetch items with Amount > 0
            var model = new RefillItemViewModel
            {
                SlotID = slot.SlotID
                
            };

            return View(model);
        }

        public async Task<string> SendMessage(Notification message)
        {
            try
            {

                var (success, returnMessage) = await _telegramService.SendMessageAsync(message,_context);

                if (success)
                {
                    return "Message sent successfully!";
                }

                return $"Failed to send the message. Error: {returnMessage}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public async Task EmptySlotAndSendMessage(int? slotId)
        {
            if (slotId == null) return;
            var slot = _context.Slots.Include(s => s.Items).FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return;
            if (slot.Items == null || slot.Items.Count == 0) return;
            try
            {
                // Log sales before deleting items
                foreach (var item in slot.Items)
                {
                    var sale = new Sale
                    {
                        ItemStorage = item,
                        SaleDate = DateTime.Now
                    };
                    _context.Sales.Add(sale);
                    await SendMessage(new Notification($"{item.Name} had been purchased of {slot.Name}", "All", 2));
                }
                slot.Items.Clear();
                var itemsList = _context.Items.Where(_ => _.SlotID == slotId);
                // Save sales first
                _context.SaveChanges();
                // Remove items and mark slot empty
                _context.Items.RemoveRange(itemsList);
                slot.IsEmpty = true;
                _context.SaveChanges();
                var message = await SendMessage(new Notification($"Slot {slot.Name} is empty please fill more items", "All", 5));
                TempData["Message"] = $"Slot {slot.Name} has been cleared and items have been logged. \n{message}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["Message"] = "An error occurred while clearing the slot.";
            }

        }

    }

}
