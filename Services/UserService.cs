using KinoAPI.Models;
using StackExchange.Redis;

namespace KinoAPI.Services;

public class UserService
{
    private IConnectionMultiplexer _redis;
    public UserService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

public async Task<User> Authenticate(string username, string password)
    {
        if (!_redis.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _redis.GetDatabase();
        var keys = _redis.GetServer("localhost", 6379).Keys();
        var keyList = keys.Where(k => k.ToString().Contains("account"));

        var _users = new List<User>();
        foreach (var key in keyList)
        {
            var userValue = await db.StringGetAsync(key);
            var tempUser = new User()
            {
                Name = key.ToString().Split('_')[1],
                Password = userValue.ToString().Split('_')[0],
                Role = userValue.ToString().Split('_')[1]
            };
            _users.Add(tempUser);
        }

        var user = _users.FirstOrDefault(x => x.Name == username);
        if(user.Password.Equals(password))
            return user;

        return null;
    }
}