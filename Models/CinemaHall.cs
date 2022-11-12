namespace KinoAPI.Models;

public class CinemaHall
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Seats { get; set; }
    public int PremiumSeats { get; set; }
    public bool IsOpen { get; set; }
    public List<MovieSession> MovieSessions { get; set; }
}
