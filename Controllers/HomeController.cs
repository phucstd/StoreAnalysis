using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StoreAnalysis.Models;
using System.Diagnostics;
using System.Drawing;
using VendingAnalysis.Analysis;

namespace StoreAnalysis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
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
                new SlotCoordinate { Name = "C2", XRange = (755, 895), YRange = (1000, 1600) },
                new SlotCoordinate { Name = "C3", XRange = (895, 1035), YRange = (1000, 1600)},
                new SlotCoordinate { Name = "C4", XRange = (1035, 1180), YRange = (1000, 1600) }
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
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
                // Save uploaded image temporarily
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".jpg");  // Ensure the image has a unique name
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Run the analysis on the uploaded image
                var json = PythonBrige.Run("deploy", tempPath);  // Pass the image path to Python
                var boundingBoxes = JsonConvert.DeserializeObject<List<Detection>>(json);
                jsonResponse = JsonConvert.SerializeObject(boundingBoxes);

                // Analyze empty slots based on bounding boxes
                emptySlots = AnalyzeSlots(boundingBoxes);

                // Draw bounding boxes on the image
                using (Image image = Image.FromFile(tempPath))
                using (var graphics = Graphics.FromImage(image))
                {
                    foreach (var box in boundingBoxes)
                    {
                        var rect = new Rectangle(box.box[0], box.box[1], box.box[2], box.box[3]);
                        graphics.DrawRectangle(Pens.Red, rect);
                    }

                    // Save processed image to byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        processedImage = memoryStream.ToArray();
                    }
                }

                // Delete temporary file
                System.IO.File.Delete(tempPath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing image: {ex.Message}");
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
    }
}
