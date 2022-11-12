using KinoAPI.Models;
using KinoAPI.Models.DTO;
using KinoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace KinoAPI.Controllers;

[ApiController]
[Route("crud/movies")]
public class MovieCRUDController : ControllerBase
{
    private MovieCRUDService _movieService;

    public MovieCRUDController(MovieCRUDService movieService)
    {
        _movieService = movieService;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] MovieDTO movieDto)
    {
        if(movieDto == null)
            return BadRequest();

        var movie = await _movieService.Add(movieDto);

        Response.Headers.Add("Content-Type", "application/json");

        return Created($"url/crud/movies/{movie.Id}", movie);
    }
    [HttpGet]
    public async Task<ActionResult<List<Movie>>> Read()
    {
        var movies = await _movieService.Get();
        if (movies.Count == 0)
            return NotFound();

        Response.Headers.Add("Content-Type", "application/json");

        return Ok(movies);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Movie>> Read([FromRoute] Guid id)
    {
        var movie = await _movieService.Get(id);
        if (movie is null)
            return NotFound();

        Response.Headers.Add("Content-Type", "application/json");

        return Ok(movie);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute]Guid id, [FromBody] MovieDTO movieDto)
    {
        if(movieDto is null)
            return BadRequest();

        await _movieService.Update(id, movieDto);

        Response.Headers.Add("Content-Type", "application/json");

        return Ok($"Updated {id}");
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<Movie>> Delete([FromRoute] Guid id)
    {
        await _movieService.Delete(id);

        Response.Headers.Add("Content-Type", "application/json");

        return NoContent();
    }
}