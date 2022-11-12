using KinoAPI.Models;
using KinoAPI.Models.DTO;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace KinoAPI.Services;

public class ForumService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public ForumService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<Guid> AddForum(User user, ForumDTO dto)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        if (dto is null)
            throw new ArgumentException("Empty dto");

        var forum = new Forum()
        {
            Id = Guid.NewGuid(),
            Author = user.Name,
            Title = dto.Title,
            Text = dto.Text
        };

        var db = _connectionMultiplexer.GetDatabase();

        var serializedForum = JsonConvert.SerializeObject(forum);
        var _ = await db.StringSetAsync(
            $"forum_{forum.Id}",
            serializedForum,
            TimeSpan.FromSeconds(600));

        return forum.Id;
    }

    public async Task<Guid> AddPost(User user, PostDTO dto, Guid forumId)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        if (dto is null)
            throw new ArgumentException("Empty dto");

        var post = new Post()
        {
            Id = Guid.NewGuid(),
            Author = user.Name,
            Text = dto.Text,
            ForumId = forumId
        };

        var db = _connectionMultiplexer.GetDatabase();

        var serializedPost = JsonConvert.SerializeObject(post);
        var _ = await db.StringSetAsync(
            $"post_{post.Id}",
            serializedPost,
            TimeSpan.FromSeconds(600));

        return post.Id;
    }

    public async Task<Forum> GetForum(Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();

        var rawForum = await db.StringGetAsync($"forum_{id}");
        if (rawForum.IsNull)
            throw new KeyNotFoundException("Forum not found");

        var forum = JsonConvert.DeserializeObject<Forum>(rawForum);

        return forum;
    }
    public async Task<Post> GetPost(Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();

        var rawPost = await db.StringGetAsync($"post_{id}");
        if (rawPost.IsNull)
            throw new KeyNotFoundException("Post not found");

        var post = JsonConvert.DeserializeObject<Post>(rawPost);

        return post;
    }
    public async Task<List<Forum>> GetForum()
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();

        var keys = _connectionMultiplexer.GetServer("localhost", 6379).Keys();
        var keyList = keys.Where(k => k.ToString().Contains("forum"));

        var forumList = new List<Forum>();
        foreach (var key in keyList)
        {
            var rawForum = await db.StringGetAsync(key);
            if (!rawForum.IsNull)
                forumList.Add(JsonConvert.DeserializeObject<Forum>(rawForum));
        }

        return forumList;
    }
    public async Task<List<Post>> GetForumPosts(Guid forumId)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();

        var keys = _connectionMultiplexer.GetServer("localhost", 6379).Keys();
        var keyList = keys.Where(k => k.ToString().Contains("post"));

        var postList = new List<Post>();
        foreach (var key in keyList)
        {
            var rawPost = await db.StringGetAsync(key);
            if (!rawPost.IsNull)
            {
                var post = JsonConvert.DeserializeObject<Post>(rawPost);
                if(post.ForumId == forumId)
                    postList.Add(post);
            }               
        }

        return postList;
    }
    public async Task<Guid> UpdateForum(User user, Guid id, ForumDTO dto)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        if (dto is null)
            throw new ArgumentException("Empty dto");

        var db = _connectionMultiplexer.GetDatabase();
        if (!db.KeyExists($"forum_{id}"))
        {
            throw new KeyNotFoundException("Forum not found");
        }

        var rawForum= await db.StringGetAsync($"forum_{id}");
        if (rawForum.IsNull)
            throw new KeyNotFoundException("Forum not found");

        var dbForum = JsonConvert.DeserializeObject<Forum>(rawForum);

        if (dbForum.Author != user.Name & user.Role != "admin")
            throw new UnauthorizedAccessException("Not priveleges for action");

        dbForum.Title = dto.Title;
        dbForum.Text = dto.Text;

        var serializedForum = JsonConvert.SerializeObject(dbForum);
        var _ = await db.StringSetAsync(
            $"forum_{dbForum.Id}",
            serializedForum,
            TimeSpan.FromSeconds(600));

        return dbForum.Id;
    }

    public async Task<Guid> UpdatePost(User user, Guid id, PostDTO dto)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        if (dto is null)
            throw new ArgumentException("Empty dto");

        var db = _connectionMultiplexer.GetDatabase();
        if (!db.KeyExists($"post_{id}"))
        {
            throw new KeyNotFoundException("Post not found");
        }

        var rawPost = await db.StringGetAsync($"post_{id}");
        if (rawPost.IsNull)
            throw new KeyNotFoundException("Post not found");

        var dbPost = JsonConvert.DeserializeObject<Post>(rawPost);

        if (dbPost.Author != user.Name & user.Role != "admin")
            throw new UnauthorizedAccessException("Not priveleges for action");

        dbPost.Text = dto.Text;

        var serializedPost = JsonConvert.SerializeObject(dbPost);
        var _ = await db.StringSetAsync(
            $"post_{dbPost.Id}",
            serializedPost,
            TimeSpan.FromSeconds(600));

        return dbPost.Id;
    }

    public async Task DeleteForum(User user, Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();
        if (!db.KeyExists($"forum_{id}"))
        {
            throw new KeyNotFoundException("Forum not found");
        }

        if(user.Role != "admin")
            throw new UnauthorizedAccessException("Not priveleges for action");

        await db.KeyDeleteAsync($"forum_{id}");
    }

    public async Task DeletePost(User user, Guid id)
    {
        if (!_connectionMultiplexer.IsConnected)
            throw new RedisException($"Can't connect to redis");

        var db = _connectionMultiplexer.GetDatabase();
        if (!db.KeyExists($"post_{id}"))
        {
            throw new KeyNotFoundException("Post not found");
        }

        if (user.Role != "admin")
            throw new UnauthorizedAccessException("Not priveleges for action");

        await db.KeyDeleteAsync($"post_{id}");
    }
}
