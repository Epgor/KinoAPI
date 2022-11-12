
using KinoAPI.Models;
using StackExchange.Redis;

namespace KinoAPI
{
    public class SystemAccountSeeder
    {
        private IConnectionMultiplexer _redis;
        public SystemAccountSeeder(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }
        public async void Seed()
        {
            if (_redis.IsConnected)
            {
                var db = _redis.GetDatabase();
                var users = GetAccounts();
                foreach(var user in users)
                {
                    var _ = await db.StringSetAsync(
                        $"account_{user.Name}",
                        $"{user.Password}_{user.Role}",
                        TimeSpan.FromSeconds(1800));
                }
            }
        }
        public List<User> GetAccounts()
        {
            var accounts = new List<User>()
            {
                new User()
                {
                    Name = "admin",
                    Password = "admin",
                    Role = "admin"
                },
                new User()
                {
                    Name = "user",
                    Password= "user",
                    Role = "user"
                }
            };
            return accounts;
        }    
    }
}
