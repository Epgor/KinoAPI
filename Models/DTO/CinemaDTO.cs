namespace KinoAPI.Models.DTO;

public class CinemaDTO
{
    public string Name = "Super Kino";
    
    public List<ScreeningDay> ScreeningDays { get; set; }
}
