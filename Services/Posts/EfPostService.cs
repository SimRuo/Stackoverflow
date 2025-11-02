using Stackoverflow.Data;
using Stackoverflow.Repositories.Posts;

namespace Stackoverflow.Services.Posts
{
    public sealed class EfPostService(IPostRepository repo) : IPostService
    {
        public Task<Post?> GetByIdAsync(int id, CancellationToken ct = default) =>
            repo.GetByIdAsync(id, ct);

        public Task<IEnumerable<Post>> Get100LatestAsync(CancellationToken ct = default) =>
            repo.Get100LatestAsync(ct);

        public Task<IEnumerable<Post>> GetAboveReuputation(int reputation, CancellationToken ct = default) =>
            repo.GetAboveReuputation(reputation, ct);

        public Task<IEnumerable<Post>> GetBetweenDates(DateTime fromDate, DateTime toDate, CancellationToken ct = default) =>
            repo.GetBetweenDates(fromDate, toDate, ct);

        public Task<int> GetPostCount(CancellationToken ct = default) =>
            repo.GetPostCount(ct);

        public Task<IEnumerable<string>> GetAllTags(CancellationToken ct = default) =>
            repo.GetAllTags(ct);
    }
}
