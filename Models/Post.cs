
namespace KinoAPI.Models;

public class Post
{
    public Guid Id { get; set; }
    public string Author { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public string Text { get; set; }
    public Guid ForumId { get; set; }
}
