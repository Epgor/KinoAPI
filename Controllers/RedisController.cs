using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace KinoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RedisController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;
    public RedisController(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    [Route("HealthCheck")]
    [HttpGet]
    public async Task<ActionResult> HealtCheck()
    {
        if(!_redis.IsConnected)
            return BadRequest("Database not found");

        var dataBase = _redis.GetDatabase();
        var pong = await dataBase.PingAsync();
        
        return Ok($"Database is ready\n Ping:{pong.TotalMilliseconds}ms");
    }

}
