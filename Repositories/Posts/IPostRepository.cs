using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stackoverflow.Data;

namespace Stackoverflow.Repositories.Posts
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Post>> Get100LatestAsync(CancellationToken ct = default);
        Task<IEnumerable<Post>> GetAboveReuputation(int reputation, CancellationToken ct = default);
        Task<IEnumerable<Post>> GetBetweenDates(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
        Task<int> GetPostCount(CancellationToken ct = default);
        Task<IEnumerable<string>> GetAllTags(CancellationToken ct = default);
        Task<IEnumerable<Post>> GetTopQuestionsByScoreAsync(int minScore, int take = 100, CancellationToken ct = default);
        Task<IEnumerable<Comment>> GetCommentsForPostAsync(int postId, CancellationToken ct = default);
        Task<IEnumerable<Post>> GetAnswersForQuestionAsync(int questionId, CancellationToken ct = default);

    }
}