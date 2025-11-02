using Microsoft.AspNetCore.Mvc;
using Stackoverflow.Data;
using Stackoverflow.Services.Posts;

namespace Stackoverflow.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController(
        EfPostService efService,
        DapperPostService dapperService) : ControllerBase
    {
        // --- SINGLE POST BY ID ---
        [HttpGet("{id:int}/ef")]
        public async Task<ActionResult<Post>> GetByIdEf(int id, CancellationToken ct)
        {
            var post = await efService.GetByIdAsync(id, ct);
            return post is null ? NotFound() : Ok(post);
        }

        [HttpGet("{id:int}/dapper")]
        public async Task<ActionResult<Post>> GetByIdDapper(int id, CancellationToken ct)
        {
            var post = await dapperService.GetByIdAsync(id, ct);
            return post is null ? NotFound() : Ok(post);
        }

        // --- 100 LATEST POSTS ---
        [HttpGet("latest/ef")]
        public async Task<ActionResult<IEnumerable<Post>>> Get100LatestEf(CancellationToken ct)
        {
            var posts = await efService.Get100LatestAsync(ct);
            return Ok(posts);
        }

        [HttpGet("latest/dapper")]
        public async Task<ActionResult<IEnumerable<Post>>> Get100LatestDapper(CancellationToken ct)
        {
            var posts = await dapperService.Get100LatestAsync(ct);
            return Ok(posts);
        }

        // --- POSTS ABOVE REPUTATION (uses Score column) ---
        [HttpGet("score/{minScore:int}/ef")]
        public async Task<ActionResult<IEnumerable<Post>>> GetAboveReputationEf(int minScore, CancellationToken ct)
        {
            var posts = await efService.GetAboveReuputation(minScore, ct);
            return Ok(posts);
        }

        [HttpGet("score/{minScore:int}/dapper")]
        public async Task<ActionResult<IEnumerable<Post>>> GetAboveReputationDapper(int minScore, CancellationToken ct)
        {
            var posts = await dapperService.GetAboveReuputation(minScore, ct);
            return Ok(posts);
        }

        // --- POSTS BETWEEN DATES ---
        [HttpGet("between/ef")]
        public async Task<ActionResult<IEnumerable<Post>>> GetBetweenDatesEf(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken ct)
        {
            var posts = await efService.GetBetweenDates(from, to, ct);
            return Ok(posts);
        }

        [HttpGet("between/dapper")]
        public async Task<ActionResult<IEnumerable<Post>>> GetBetweenDatesDapper(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            CancellationToken ct)
        {
            var posts = await dapperService.GetBetweenDates(from, to, ct);
            return Ok(posts);
        }

        // --- TOTAL POST COUNT ---
        [HttpGet("count/ef")]
        public async Task<ActionResult<int>> GetPostCountEf(CancellationToken ct)
        {
            var count = await efService.GetPostCount(ct);
            return Ok(count);
        }

        [HttpGet("count/dapper")]
        public async Task<ActionResult<int>> GetPostCountDapper(CancellationToken ct)
        {
            var count = await dapperService.GetPostCount(ct);
            return Ok(count);
        }

        // --- UNIQUE TAGS ---
        [HttpGet("tags/ef")]
        public async Task<ActionResult<IEnumerable<string>>> GetTagsEf(CancellationToken ct)
        {
            var tags = await efService.GetAllTags(ct);
            return Ok(tags);
        }

        [HttpGet("tags/dapper")]
        public async Task<ActionResult<IEnumerable<string>>> GetTagsDapper(CancellationToken ct)
        {
            var tags = await dapperService.GetAllTags(ct);
            return Ok(tags);
        }
    }
}
