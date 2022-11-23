using KinoAPI.Models;
using KinoAPI.Models.DTO;
using StackExchange.Redis;
using Newtonsoft.Json;
namespace KinoAPI.Services;

public static class CinemaSeedingService
{

    private static IDatabase _redis;
    private static string _filePath = "C:\\Users\\user\\Desktop\\Pliki_Studia\\projekty\\KinoAPI\\exampleSeedData.json";
    public static void ConfigureSeeder(IConnectionMultiplexer redis, string storeKey)
    {
        if (!redis.IsConnected)
            throw new RedisException("Redis not present");

        _redis = redis.GetDatabase();
    }

    public async static void SeedDataBase(int howManyDays = 5)
    {
        var __ = await HealthCheck();

        SendData(howManyDays);

        var cinema = await GetData();

        Console.Write(cinema);

        await SaveToFile(cinema, _filePath);
    }

    public async static Task<CinemaDTO> GetData()
    {
        var gotCinema = await _redis.StringGetAsync($"cinemaKey");

        Console.WriteLine(gotCinema);

        var deserializedCinema = JsonConvert.DeserializeObject<CinemaDTO>(gotCinema);

        return deserializedCinema;
    }
    private async static void SendData(int days = 5)
    {
        var screeningDays = new List<ScreeningDay>();
        for (int day = 0; day < days; day++)
        {
            var seed = 666 + day * 1000;
            Random random = new Random(/*seed*/);
            seed += random.Next();
            DateTime screenDate = DateTime.Today.AddDays(random.Next(-60, 60));

            var screeningDay = new ScreeningDay().Generate(seed, screenDate);
            screeningDays.Add(screeningDay);
        }
        var cinema = new CinemaDTO()
        {
            ScreeningDays = screeningDays
        };
        var serializedCinema = JsonConvert.SerializeObject(cinema, Formatting.Indented);
        var _ = await _redis.StringSetAsync(
            $"cinemaKey",
            serializedCinema,
            TimeSpan.FromMinutes(60));
    }
    private async static Task<double> HealthCheck()
    {
        var pong = await _redis.PingAsync();

        Console.WriteLine($"Database is ready\n Ping:{pong.TotalMilliseconds}ms");
        return pong.TotalMilliseconds;
    }
    private static async Task<int> SaveToFile(CinemaDTO cinema, string fileName)
    {
        try
        {
            var serializedDays = JsonConvert.SerializeObject(cinema, Formatting.Indented);

            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(serializedDays);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return 1;
        }
        return 0;
    }
}
