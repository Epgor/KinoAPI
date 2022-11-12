using KinoAPI.Models;
using KinoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace KinoAPI.Controllers;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private MovieService _movieService;
    public MovieController(MovieService movieService)
    {
        _movieService = movieService;
    }
    [HttpGet]
    public async Task<ActionResult<List<Movie>>> GetMovies()
    {
        var movies = await _movieService.Get();
        if(movies.Count == 0)
            return NotFound();

        return Ok(movies);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<List<Movie>>> GetMovie([FromRoute]Guid id)
    {
        var movie = await _movieService.Get(id);
        if (movie is null)
            return NotFound();

        return Ok(movie);
    }
}