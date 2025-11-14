using System.Globalization;
using System.Xml.Linq;
using TestProject.ViewModels;

namespace TestProject.Services
{
    /// <summary>
    /// Service for fetching and parsing external news feeds.
    /// </summary>
    public class NewsFeedService : INewsFeedService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsFeedService> logger;

        public NewsFeedService(HttpClient httpClient, ILogger<NewsFeedService> logger)
        {
            _httpClient = httpClient;
            this.logger = logger;

            // Set a reasonable timeout
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Fetches articles from provided feed URL.
        /// Safely handles errors and returns an empty list on failure.
        /// </summary>
        /// <param name="feedUrl">External feed URL. From Backoffice</param>
        /// <param name="ct">CancelationToken</param>
        /// <returns></returns>
        public async Task<IReadOnlyList<ArticleViewModel>> GetArticlesAsync(string feedUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(feedUrl))
            {
                logger.LogWarning("Feed URL is null or empty.");
                return Array.Empty<ArticleViewModel>();
            }

            try
            {
                using var resp = await _httpClient.GetAsync(feedUrl, ct);
                if (!resp.IsSuccessStatusCode)
                {
                    logger.LogWarning($"Failed to fetch feed {feedUrl}. Status: {resp.StatusCode}");

                    return Array.Empty<ArticleViewModel>();
                }

                var xml = await resp.Content.ReadAsStringAsync(ct);

                var xdoc = XDocument.Parse(xml);

                // Map XML items to ArticleViewModel
                var items = xdoc.Descendants("item")
                    .Select(MapToArticle)
                    .Where(a => a is not null)
                    .Cast<ArticleViewModel>()
                    .OrderByDescending(a => a.PublishedDateUtc ?? DateTime.MinValue)
                    .ToList();

                return items;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Exception occurred while fetching or parsing feed {feedUrl}.");
                return Array.Empty<ArticleViewModel>();
            }
        }

        /// <summary>
        /// Maps an XML element to an ArticleViewModel.
        /// </summary>
        /// <param name="item">Single XML element</param>
        /// <returns>ArticleViewModel</returns>
        private ArticleViewModel? MapToArticle(XElement item)
        {
            if (item == null)
            {
                return null;
            }

            try
            {
                var title = (string?)item.Element("title") ?? "";
                var description = (string?)item.Element("description") ?? "";
                var link = (string?)item.Element("link") ?? "";
                var imageUrl = (string?)item.Element("image") ?? "";

                DateTime? pubDate = null;
                if (DateTime.TryParse((string?)item.Element("pubDate"), CultureInfo.InvariantCulture,
                                      DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                      out var parsed))
                {
                    pubDate = parsed;
                }

                // Truncate description to 100 characters
                if (description.Length > 100)
                    description = description[..100] + "…";

                return new ArticleViewModel
                {
                    Title = title,
                    Description = description,
                    Link = link,
                    ImageUrl = imageUrl,
                    PublishedDateUtc = pubDate
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error mapping XML item to ArticleViewModel.");
                return null;
            }
        }
    }
}
