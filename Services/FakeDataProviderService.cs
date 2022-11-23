using Bogus;
using KinoAPI.Models;

namespace KinoAPI.Services;

public static class FakeDataProviderService
{
    public static Movie Generate(this Movie movie, int seed)
    {
        Randomizer.Seed = new Random(/*seed*/);
        Random random = new Random(/*seed*/);

        movie = new Faker<Movie>()
            .RuleFor(p => p.Id, (f, p) => p.Id = Guid.NewGuid())
            .RuleFor(p => p.Name, (f, p) => f.Company.CompanyName())
            .RuleFor(p => p.Genre, (f, p) => f.Music.Genre())
            .RuleFor(p => p.Length, (f, p) => p.Length = TimeSpan.FromSeconds(random.Next(3000, 10000)))
            .RuleFor(p => p.TicketPrice, (f, p) => p.TicketPrice = random.Next(10, 800)/10f)
            .RuleFor(p => p.ImageUrl, (f, p) => p.ImageUrl = "https://www.creavea.com/produits/82320-l/image-3d-divers-zen-n2-30-x-30-cm-l.jpg")
            .Generate(1)
            .First();
        return movie;
    }

    public static MovieSession Generate(this MovieSession movieSession,
                                               int seed,
                                               int occupiedSeats,
                                               int occupiedPremiumSeats,
                                               DateTime startDateTime)
    {
        Randomizer.Seed = new Random(/*seed*/);
        Random random = new Random(/*seed*/);

        movieSession = new Faker<MovieSession>()
            .RuleFor(p => p.Id, (f, p) => p.Id = Guid.NewGuid())
            .RuleFor(p => p.OccupiedSeats, (f, p) => p.OccupiedSeats = occupiedSeats)
            .RuleFor(p => p.OccupiedPremiumSeats, (f, p) => p.OccupiedPremiumSeats = occupiedPremiumSeats)
            .RuleFor(p => p.StartDateTime, (f, p) => p.StartDateTime = startDateTime)
            .RuleFor(p => p.MoviePlayed, (f, p) => p.MoviePlayed = p.MoviePlayed.Generate(seed))
            .Generate(1)
            .First();
        return movieSession;
    }

    public static CinemaHall Generate(this CinemaHall cinemaHall, int seed, DateTime dateDay, int quantity)
    {
        Randomizer.Seed = new Random(/*seed*/);
        Random random = new Random(/*seed*/);

        var totalSeats = random.Next(80, 200);

        var totalPremiumSeats = random.Next(2,totalSeats / 8);

        cinemaHall = new Faker<CinemaHall>()
            .RuleFor(p => p.Id, (f, p) => p.Id = Guid.NewGuid())
            .RuleFor(p => p.Name, (f, p) => f.Commerce.ProductName())
            .RuleFor(p => p.Seats, (f, p) => p.Seats = totalSeats)
            .RuleFor(p => p.PremiumSeats, (f, p) => p.PremiumSeats = totalPremiumSeats)
            .RuleFor(p => p.IsOpen, (f, p) => p.IsOpen = true)
            .RuleFor(p => p.MovieSessions, (f, p) => 
                p.MovieSessions = GenerateMovies(seed, dateDay, quantity, totalSeats, totalPremiumSeats))
            .Generate(1)
            .First();
        return cinemaHall;
    }

    public static ScreeningDay Generate(this ScreeningDay screeningDay, int seed, DateTime screenDate)
    {
        Randomizer.Seed = new Random(/*seed*/);
        Random random = new Random(/*seed*/);

        int moviesPerDay = 5;
        int numberOfHalls = 10;

        screeningDay = new Faker<ScreeningDay>()
            .RuleFor(p => p.Id, (f, p) => p.Id = Guid.NewGuid())
            .RuleFor(p => p.Date, (f, p) => p.Date = screenDate)
            .RuleFor(p => p.IsOpen, (f, p) => p.IsOpen = true)
            .RuleFor(p => p.IsExtraPaid, (f, p) => p.IsExtraPaid = random.Next(2) == 1)
            .RuleFor(p => p.CinemaHalls, (f, p) => 
                p.CinemaHalls = GenerateCinemaHalls(seed, screenDate, moviesPerDay, numberOfHalls))
            .Generate(1)
            .First();

        return screeningDay;
    }

    private static List<CinemaHall> GenerateCinemaHalls(int seed,
                                                        DateTime date,
                                                        int moviesPerDay,
                                                        int numberOfHalls)
    {
        var cinemaHalls = new List<CinemaHall>();

        for(int i = 0; i<numberOfHalls; i++)
        {
            var cinemaHall = new CinemaHall().Generate(seed, date, moviesPerDay);

            cinemaHalls.Add(cinemaHall);
        }
        return cinemaHalls;

    }
    private static List<MovieSession> GenerateMovies(int seed,
                                                     DateTime dateToday,
                                                     int moviesPerDay,
                                                     int totalSeats,
                                                     int totalPremiumSeats)
    {
        var movieSessions = new List<MovieSession>();

        for(int i = 0; i<moviesPerDay; i++)
        {
            var localSeed = seed + i;
            Random random = new Random(/*localSeed*/);
            localSeed += random.Next();

            var seatsTaken = random.Next(1, totalSeats);
            var premiumSeatsTaken = random.Next(1, totalPremiumSeats);

            var movieSession = new MovieSession().Generate(localSeed,
                                                           seatsTaken,
                                                           premiumSeatsTaken,
                                                           dateToday.AddHours((i+1) * 4));
            movieSessions.Add(movieSession);
        }

        return movieSessions;
    }
}
