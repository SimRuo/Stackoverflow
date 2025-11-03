using Microsoft.EntityFrameworkCore;
using Stackoverflow.Data;

namespace Stackoverflow.Repositories.Posts
{
    public sealed class EfPostRepository(AppDbContext db) : IPostRepository
    {
        public async Task<Post?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await db.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IEnumerable<Post>> Get100LatestAsync(CancellationToken ct = default) =>
            await db.Posts.AsNoTracking()
                .OrderByDescending(p => p.CreationDate)
                .Take(100)
                .ToListAsync(ct);

        public async Task<IEnumerable<Post>> GetAboveReuputation(int reputation, CancellationToken ct = default) =>
            await db.Posts.AsNoTracking()
                .Where(p => p.Score >= reputation)
                .OrderByDescending(p => p.Score)
                .Take(100)
                .ToListAsync(ct);

        public async Task<IEnumerable<Post>> GetBetweenDates(DateTime fromDate, DateTime toDate, CancellationToken ct = default) =>
            await db.Posts.AsNoTracking()
                .Where(p => p.CreationDate >= fromDate && p.CreationDate <= toDate)
                .OrderByDescending(p => p.CreationDate)
                .Take(100)
                .ToListAsync(ct);

        public async Task<int> GetPostCount(CancellationToken ct = default) =>
            await db.Posts.AsNoTracking().CountAsync(ct);

        public async Task<IEnumerable<string>> GetAllTags(CancellationToken ct = default)
        {
            var rawTags = await db.Posts.AsNoTracking()
                .Where(p => p.Tags != null)
                .Select(p => p.Tags!)
                .ToListAsync(ct);

            return rawTags
                .SelectMany(t => t.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
        public async Task<IEnumerable<Post>> GetTopQuestionsByScoreAsync(int minScore, int take = 100, CancellationToken ct = default) =>
    await db.Posts.AsNoTracking()
        .Where(p => p.PostTypeId == 1 && p.Score > minScore)
        .OrderByDescending(p => p.Score)
        .ThenByDescending(p => p.CreationDate)
        .Take(take)
        .ToListAsync(ct);

        public async Task<IEnumerable<Comment>> GetCommentsForPostAsync(int postId, CancellationToken ct = default) =>
            await db.Comments.AsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreationDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<Post>> GetAnswersForQuestionAsync(int questionId, CancellationToken ct = default) =>
            await db.Posts.AsNoTracking()
                .Where(p => p.PostTypeId == 2 && p.ParentId == questionId)
                .OrderByDescending(p => p.Score)
                .ThenBy(p => p.CreationDate)
                .ToListAsync(ct);
    }
}
