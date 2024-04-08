namespace CompetitorDetails.Models
{
    public class Competitor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Status { get; set; }

        public ICollection<ArticleDetail>? ArticleDetails { get; set; }
    }
}
