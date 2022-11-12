using KinoAPI.Models;
using KinoAPI.Models.DTO;
using KinoAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KinoAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/Forum")]
public class ForumController : ControllerBase
{
    private readonly ForumService _forumService;
    public ForumController(ForumService forumService)
    {
        _forumService = forumService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateForum([FromBody]ForumDTO dto)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null || dto is null)
            return BadRequest();

        var id = await _forumService.AddForum(user, dto);

        return Ok(id);
    }
    [HttpGet]
    public async Task<ActionResult<Forum>> ReadAllForum()
    {
        var forums = await _forumService.GetForum();

        return Ok(forums);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<List<Forum>>> ReadForum([FromRoute]Guid id)
    {
        var forum = await _forumService.GetForum(id);

        return Ok(forum);
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<Guid>> UpdateForum([FromRoute]Guid id, [FromBody]ForumDTO dto)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null || dto is null)
            return BadRequest();

        var forumId = await _forumService.UpdateForum(user, id, dto);

        return Ok(forumId);
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<Guid>> DeleteForum([FromRoute] Guid id)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null)
            return BadRequest();

        var posts = await _forumService.GetForumPosts(id);

        foreach (var post in posts)
        {
            await _forumService.DeletePost(user, post.Id);
        }

        await _forumService.DeleteForum(user, id);

        return NoContent();
    }
    [HttpPost("{forumId}/Post")]
    public async Task<ActionResult<Guid>> CreatePost([FromRoute]Guid forumId, [FromBody] PostDTO dto)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null || dto is null)
            return BadRequest();

        var id = await _forumService.AddPost(user, dto, forumId);

        return Ok(id);
    }
    [HttpGet("{forumId}/Post")]
    public async Task<ActionResult<Post>> ReadAllPost([FromRoute] Guid forumId)
    {
        var posts = await _forumService.GetForumPosts(forumId);

        return Ok(posts);
    }
    [HttpGet("{forumId}/Post/{postId}")]
    public async Task<ActionResult<List<Post>>> ReadPost([FromRoute] Guid forumId, Guid postId)
    {
        var post = await _forumService.GetForum(postId);

        return Ok(post);
    }
    [HttpPut("{forumId}/Post/{postId}")]
    public async Task<ActionResult<Guid>> UpdatePost([FromRoute] Guid forumId, Guid postId, [FromBody] PostDTO dto)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null || dto is null)
            return BadRequest();

        var _ = await _forumService.UpdatePost(user, postId, dto);

        return Ok(postId);
    }
    [HttpDelete("{forumId}/Post/{postId}")]
    public async Task<ActionResult<Guid>> DeletePost([FromRoute] Guid forumId, Guid postId)
    {
        var user = UserResolvingService.Resolve(Request.HttpContext);
        if (user == null)
            return BadRequest();

        await _forumService.DeletePost(user, postId);

        return NoContent();
    }
}
