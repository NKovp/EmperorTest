using TestProject.ViewModels;

namespace TestProject.Services
{
    public interface INewsFeedService
    {
        Task<IReadOnlyList<ArticleViewModel>> GetArticlesAsync(string feedUrl, CancellationToken ct);
    }
}
