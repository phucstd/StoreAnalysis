using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StoreAnalysis.Data;
using StoreAnalysis.Models;
using StoreAnalysis.Script;
using System.Diagnostics;
using System.Drawing;
using VendingAnalysis.Analysis;
using static Python.Runtime.TypeSpec;

namespace StoreAnalysis.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly StoreAnalysisContext _context;
        private readonly TelegramService _telegramService;
        public HomeController(StoreAnalysisContext context, TelegramService telegramService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _telegramService = telegramService;
            _webHostEnvironment = webHostEnvironment;
        }
        public static List<SlotCoordinate> GetSlots()
        {
            return new List<SlotCoordinate>
            {
                new SlotCoordinate { Name = "A1", XRange = (615, 755), YRange = (400, 800) },
                new SlotCoordinate { Name = "A2", XRange = (755, 895), YRange = (400, 800) },
                new SlotCoordinate { Name = "A3", XRange = (895, 1035), YRange = (400, 800) },
                new SlotCoordinate { Name = "A4", XRange = (1035, 1180), YRange = (400, 800) },
                new SlotCoordinate { Name = "B1", XRange = (615, 755), YRange = (800, 1200) },
                new SlotCoordinate { Name = "B2", XRange = (755, 895), YRange = (800, 1200) },
                new SlotCoordinate { Name = "B3", XRange = (895, 1035), YRange = (800, 1200) },
                new SlotCoordinate { Name = "B4", XRange = (1035, 1180), YRange = (800, 1200) },
                new SlotCoordinate { Name = "C1", XRange = (615, 755), YRange = (1200, 1600) },
                new SlotCoordinate { Name = "C2", XRange = (755, 895), YRange = (1200, 1600) },
                new SlotCoordinate { Name = "C3", XRange = (895, 1035), YRange = (1200, 1600)},
                new SlotCoordinate { Name = "C4", XRange = (1035, 1180), YRange = (1200, 1600) }
            };
        }

        private List<string> AnalyzeSlots(List<Detection> detections)
        {
            var slots = GetSlots();
            var emptySlots = new List<string>();

            // Iterate through each slot to check if it has any detection
            foreach (var slot in slots)
            {
                bool isEmpty = detections.All(detection =>
                {
                    int x = detection.box[0];
                    int y = detection.box[1];

                    // Check if the detection is outside the slot range
                    return x < slot.XRange.Min || x > slot.XRange.Max ||
                           y < slot.YRange.Min || y > slot.YRange.Max;
                });

                // If the slot is empty (no detection within its range), add it to the list
                if (isEmpty)
                {
                    emptySlots.Add(slot.Name);
                }
            }

            return emptySlots;
        }
        

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult AnalyzeImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            string jsonResponse;
            byte[] processedImage;
            List<string> emptySlots = new List<string>();

            try
            {
                // Define paths for saving the uploaded and processed images
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                string processedFolder = Path.Combine(_webHostEnvironment.WebRootPath, "processed");

                // Ensure directories exist
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                if (!Directory.Exists(processedFolder)) Directory.CreateDirectory(processedFolder);

                // Generate unique file names
                string inputFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string processedFileName = Guid.NewGuid().ToString() + ".png";

                string inputFilePath = Path.Combine(uploadsFolder, inputFileName);
                string processedFilePath = Path.Combine(processedFolder, processedFileName);

                // Save uploaded image
                using (var stream = new FileStream(inputFilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Run the analysis on the uploaded image
                var json = PythonBrige.Run("deploy", inputFilePath);  // Pass the image path to Python
                var boundingBoxes = JsonConvert.DeserializeObject<List<Detection>>(json);
                jsonResponse = JsonConvert.SerializeObject(boundingBoxes);

                // Analyze empty slots based on bounding boxes
                emptySlots = AnalyzeSlots(boundingBoxes);

                var slotsId = emptySlots.Select(name => _context.Slots.FirstOrDefault(_ => _.Name.Equals(name))?.SlotID).ToList();
                if (slotsId != null)
                {
                    var list = slotsId.ToList();
                    string message = "";
                    foreach (var slot in list)
                    {
                        EmptySlot(slot);
                        var slotName = _context.Slots.FirstOrDefault(s => s.SlotID == slot)?.Name;
                        message += message.Equals("") ? "" : "," + $"{slotName}";
                    }
                    if(!string.IsNullOrEmpty(message))
                    {
                        TempData["Message"] = $"Slot {message} has been cleared and items have been logged.\n";

                    }
                }

                // Draw bounding boxes on the image
                using (Image image = Image.FromFile(inputFilePath))
                using (var graphics = Graphics.FromImage(image))
                {
                    foreach (var box in boundingBoxes)
                    {
                        var rect = new Rectangle(box.box[0], box.box[1], box.box[2], box.box[3]);
                        graphics.DrawRectangle(Pens.Red, rect);
                    }

                    // Save the processed image
                    image.Save(processedFilePath, System.Drawing.Imaging.ImageFormat.Png);

                    // Save processed image to byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        processedImage = memoryStream.ToArray();
                    }
                }
                _context.AnalysisImages.Add(new AnalysisImage() { AnalyzedImagePath = processedFileName, CameraImagePath = inputFileName, Date = DateTime.Now });
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image: {ex.Message}");
                return StatusCode(500, "Error processing the image.");
            }

            // Return JSON response with bounding boxes, empty slots, and the processed image
            return Json(new
            {
                boundingBoxes = jsonResponse,
                emptySlots = emptySlots,  // Include the list of empty slots
                image = Convert.ToBase64String(processedImage)  // Return image as base64 string
            });
        }



        public async void EmptySlot(int? slotId)
        {
            if(slotId == null) return;
            var slot = _context.Slots.FirstOrDefault(s => s.SlotID == slotId);
            if (slot == null) return;
            var items = _context.GetItemsOnSlot(slotId.Value);
            if (items == null || items.Count == 0) return;
            // Log sales before deleting items
            foreach (var item in items)
            {
                var sale = new Sale
                {
                    ItemStorage = item,
                    SaleDate = DateTime.Now
                };
                _context.Sales.Add(sale);
            }
            var itemsList = _context.Items.Where(_ => _.SlotID == slotId);
            // Save sales first
            _context.SaveChanges();
            // Remove items and mark slot empty
            _context.Items.RemoveRange(itemsList);
            slot.IsEmpty = true;
            _context.SaveChanges();
            var message = await SendMessage(new Notification($"Slot {slot.Name} is empty please fill more items", "Employee" , 5));
            
        }

        public async Task<string> SendMessage(Notification message)
        {
            try
            {
                
                var (success, returnMessage) = await _telegramService.SendMessageAsync(message, _context);

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
    }
}
