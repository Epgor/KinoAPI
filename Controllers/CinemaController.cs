using KinoAPI.Models;
using KinoAPI.Models.DTO;
using KinoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace KinoAPI.Controllers;

[ApiController]
[Route("api/cinema")]
public class CinemaController : ControllerBase
{
    private MovieService _movieService;
    public CinemaController(MovieService movieService)
    {
        _movieService = movieService;
    }
    [HttpGet]
    public async Task<ActionResult<CinemaDTO>> Get()
    {
        var cinema = await _movieService.GetCinema();
        Response.Headers["Content-Type"] = "application/json";
        return Ok(cinema);
    }
}

