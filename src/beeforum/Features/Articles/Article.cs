using System.Collections.Generic;

namespace beeforum.Features.Articles
{
    public class ArticleEnvelope
    {
        public ArticleEnvelope(Domain.Article article)
        {
            Article = article;
        }

        public Domain.Article Article { get; }
    }

    public class ArticlesEnvelope
    {
        public List<Domain.Article> Articles { get; set; }

        public int ArticlesCount { get; set; }
    }
}