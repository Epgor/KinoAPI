using KinoAPI.Models;
using KinoAPI.Models.DTO;
using StackExchange.Redis;

namespace KinoAPI.Services;

public class MovieService
{
    private IConnectionMultiplexer _connectionMultiplexer;
    private List<Movie> _movieList = new List<Movie>();
    public MovieService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Movie> Get(Guid guid)
    {
        if (_movieList.Count == 0)
            await LoadDefaultData();

        var movie = _movieList.First(g => g.Id == guid);

        if (movie is null)
            return null;

        return movie;
    }

    public async Task<List<Movie>> Get()
    {
        if (_movieList.Count == 0)
            await LoadDefaultData();

        //GetAllMovies();
        return _movieList;
    }

    public async Task<CinemaDTO> GetCinema()
    {
       return await CinemaSeedingService.GetData();
    }

    public async Task LoadDefaultData()
    {
        var cinema = await CinemaSeedingService.GetData();
        GetAllMovies(cinema);
    }
    private void GetAllMovies(CinemaDTO cinema)
    {
        foreach (var day in cinema.ScreeningDays)
        {
            ForHalls(day);
        }
    }   
    private void ForSessions(CinemaHall hall)
    {
        foreach (var session in hall.MovieSessions)
        {
            _movieList.Add(session.MoviePlayed);
        }
    }
    private void ForHalls(ScreeningDay day)
    {
        foreach (var hall in day.CinemaHalls)
        {
            ForSessions(hall);
        }
    }


}
