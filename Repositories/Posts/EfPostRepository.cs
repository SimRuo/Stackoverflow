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
    }
}
