using StoreAnalysis.Data;
using StoreAnalysis.Models;
using System.Text;

namespace StoreAnalysis.Script
{
    public class TelegramService
    {
        private readonly HttpClient _httpClient;
        private readonly string _botToken;
        private readonly string _chatId;
        private readonly string _apiUrl = "https://api.telegram.org/bot";

        public TelegramService(HttpClient httpClient, string botToken, string chatId)
        {
            _httpClient = httpClient;
            _botToken = botToken;
            _chatId = chatId;
        }

        public async Task<(bool success, string message)> SendMessageAsync(Notification message, StoreAnalysisContext context)
        {
            try
            {
                var url = $"{_apiUrl}{_botToken}/sendMessage";

                var payload = new
                {
                    chat_id = _chatId,
                    text = $"[{message.Role}] - {message.Content}"
                };

                var jsonContent = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response Content: {responseContent}");
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Notifications.Add(message);
                    await context.SaveChangesAsync();
                }
                return (response.IsSuccessStatusCode, responseContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return (false, ex.Message);
            }
        }
    }

}
