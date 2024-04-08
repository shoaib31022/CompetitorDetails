namespace CompetitorDetails.Models
{
    public class ArticleDetail
    {
        public int Id { get; set; }
        public string? ArticleTitle { get; set; }
        public string? ArticleUrl { get; set; }
        public string? ArticleTime { get; set; }
        public Guid BrandId { get; set; }
        public Competitor? Competitors { get; set; }
    }
}
