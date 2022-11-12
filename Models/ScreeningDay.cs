namespace KinoAPI.Models;

public class ScreeningDay
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsOpen { get; set; }
    public bool IsExtraPaid { get; set; }
    public List<CinemaHall> CinemaHalls { get; set; }

}
