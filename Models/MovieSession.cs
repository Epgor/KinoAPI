namespace KinoAPI.Models;

public class MovieSession
{
    public Guid Id { get; set; }
    public int OccupiedSeats { get; set; }
    public int OccupiedPremiumSeats { get; set;}
    public DateTime StartDateTime { get; set; }
    public Movie MoviePlayed { get; set; }
}
