namespace KinoAPI.Models;

public class Forum
{
    public Guid Id { get; set; }
    public string Author { get; set; }
    public DateTime Data { get; set; } = DateTime.Now;
    public string Title { get; set; }
    public string Text { get; set; }
}