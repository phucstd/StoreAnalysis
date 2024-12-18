namespace StoreAnalysis.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedDate { get; set; }

        public Notification(string content, string role, int priority)
        {
            Content = content;
            Role = role;
            Priority = priority;
            CreatedDate = DateTime.Now;
        }   
    }
}
