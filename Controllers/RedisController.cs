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
    public async Task<ActionResult> HealtCheck(/*[FromQuery]int responseType = 0*/)
    {
        int responseType = 0;
        if(!_redis.IsConnected)
            return BadRequest("Database not found");

        var dataBase = _redis.GetDatabase();
        var pong = await dataBase.PingAsync();

        if(responseType == 0)
        {
            Response.Headers["Content-Type"] = "text/plain";
            return Ok($"Database is ready\n Ping:{pong.TotalMilliseconds}ms");
        }   
        else
        {
            var response = new ResponseJson() { response = $"Database is ready\n Ping:{pong.TotalMilliseconds}ms" };
            Response.Headers["Content-Type"] = "application/json";
            return Ok(response);
        }
            

    }

}

public class ResponseJson
{
    public ResponseJson()
    {
    }
    public string response { get; set; }
}
