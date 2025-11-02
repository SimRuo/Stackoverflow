using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stackoverflow.Data;

namespace Stackoverflow.Services.Posts
{
    public interface IPostService
    {
        Task<Post?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Post>> Get100LatestAsync(CancellationToken ct = default);
        Task<IEnumerable<Post>> GetAboveReuputation(int reputation, CancellationToken ct = default);
        Task<IEnumerable<Post>> GetBetweenDates(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
        Task<int> GetPostCount(CancellationToken ct = default);
        Task<IEnumerable<string>> GetAllTags(CancellationToken ct = default);
    }
}