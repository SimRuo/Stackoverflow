using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Stackoverflow.Data;
namespace Stackoverflow.Repositories.Posts
{
    public sealed class DapperPostRepository(SqlConnection connection) : IPostRepository
    {
        private async Task EnsureOpenAsync(CancellationToken ct)
        {
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(ct);
        }

        public async Task<Post?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT TOP 1 * FROM Posts WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<Post>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Post>> Get100LatestAsync(CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT TOP 100 * FROM Posts ORDER BY CreationDate DESC";
            return await connection.QueryAsync<Post>(sql);
        }

        public async Task<IEnumerable<Post>> GetAboveReuputation(int reputation, CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT TOP 100 * FROM Posts WHERE Score >= @Reputation ORDER BY Score DESC";
            return await connection.QueryAsync<Post>(sql, new { Reputation = reputation });
        }

        public async Task<IEnumerable<Post>> GetBetweenDates(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT TOP 100 * FROM Posts WHERE CreationDate BETWEEN @FromDate AND @ToDate ORDER BY CreationDate DESC";
            return await connection.QueryAsync<Post>(sql, new { FromDate = fromDate, ToDate = toDate });
        }

        public async Task<int> GetPostCount(CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT COUNT(*) FROM Posts";
            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task<IEnumerable<string>> GetAllTags(CancellationToken ct = default)
        {
            await EnsureOpenAsync(ct);
            const string sql = "SELECT Tags FROM Posts WHERE Tags IS NOT NULL";
            var rawTags = await connection.QueryAsync<string>(sql);

            // Flatten tag strings like <c#><sql><asp.net> → ["c#", "sql", "asp.net"]
            return rawTags
                .SelectMany(t => t.Split(new[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }
    }
}
