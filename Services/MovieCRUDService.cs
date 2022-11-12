using KinoAPI.Models;
using KinoAPI.Models.DTO;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace KinoAPI.Services;

public class MovieCRUDService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public MovieCRUDService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Movie> Get(Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();
       
        var rawMovie = await db.StringGetAsync($"key_{id}");

        if (rawMovie.IsNull)
            throw new KeyNotFoundException("Movie not found");

        var movie = JsonConvert.DeserializeObject<Movie>(rawMovie);

        return movie;
    }

    public async Task<List<Movie>> Get()
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();
        var keys = _connectionMultiplexer.GetServer("localhost", 6379).Keys();
        var keyList = keys.Where(k => k.ToString().Contains("key"));

        var movieList = new List<Movie>();
        foreach (var key in keyList)
        {
            var rawMovie = await db.StringGetAsync(key);
            if(!rawMovie.IsNull)
                movieList.Add(JsonConvert.DeserializeObject<Movie>(rawMovie));
        }

        return movieList;
    }

    public async Task<Movie> Add(MovieDTO movieDto)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");
        var db = _connectionMultiplexer.GetDatabase();

        var movie = new Movie()
        {
            Id = Guid.NewGuid(),
            Name = movieDto.Name,
            Genre = movieDto.Genre,
            Length = TimeSpan.FromMinutes(movieDto.LengthInMinutes),
            TicketPrice = movieDto.TicketPrice,
            ImageUrl = movieDto.ImageUrl,
        };

        var serializedMovie = JsonConvert.SerializeObject(movie);
        var _ = await db.StringSetAsync(
            $"key_{movie.Id}",
            serializedMovie,
            TimeSpan.FromSeconds(600));

        return movie;
    }

    public async Task Delete(Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");
        var db = _connectionMultiplexer.GetDatabase();
        if (!db.KeyExists($"key_{id}"))
        {
            throw new KeyNotFoundException("Movie not found");
        }

        await db.KeyDeleteAsync($"key_{id}");
    }

    public async Task Update(Guid id, MovieDTO movieDto)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");
        var db = _connectionMultiplexer.GetDatabase();

        if (!db.KeyExists($"key_{id}"))
        {
            throw new KeyNotFoundException("Movie not found");
        }
        await db.KeyDeleteAsync($"key_{id}");

        var movie = new Movie()
        {
            Id = id,
            Name = movieDto.Name,
            Genre = movieDto.Genre,
            Length = TimeSpan.FromMinutes(movieDto.LengthInMinutes),
            TicketPrice = movieDto.TicketPrice,
            ImageUrl = movieDto.ImageUrl,
        };
        var serializedMovie = JsonConvert.SerializeObject(movie);
        var _ = await db.StringSetAsync(
            $"key_{movie.Id}",
            serializedMovie,
            TimeSpan.FromSeconds(600));
    }
}
