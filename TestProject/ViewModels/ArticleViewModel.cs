using MessagePack.Formatters;

namespace TestProject.ViewModels
{
    public class ArticleViewModel
    {
        public string Title { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? PublishedDateUtc { get; set; }
        public string Link { get; set; } = "";
    }
}