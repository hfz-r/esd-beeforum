using System.Linq;
using System.Threading.Tasks;
using beeforum.Features.Articles;
using Xunit;

namespace beeforum.IntegrationTests.Features.Articles
{
    public class CreateTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Create_Article()
        {
            var command = new Create.Command
            {
                Article = new Create.ArticleData
                {
                    Title = "Test article 666",
                    Description = "Description of the test article",
                    Body = "Body of the test article",
                    TagList = new[] {"tag1", "tag2"}
                }
            };

            var article = await ArticleHelpers.CreateArticle(this, command);

            Assert.NotNull(article);
            Assert.Equal(article.Title, command.Article.Title);
            Assert.Equal(article.TagList.Count, command.Article.TagList.Length);
        }
    }
}