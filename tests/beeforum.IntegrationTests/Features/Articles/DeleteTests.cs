using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Features.Articles;
using beeforum.IntegrationTests.Features.Comments;
using beeforum.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace beeforum.IntegrationTests.Features.Articles
{
    public class DeleteTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Delete_Article()
        {
            var createCommand = new Create.Command
            {
                Article = new Create.ArticleData
                {
                    Title = "Test article 666",
                    Description = "Description of the test article",
                    Body = "Body of the test article"
                }
            };

            var createdArticle = await ArticleHelpers.CreateArticle(this, createCommand);
            var slug = createdArticle.Slug;

            var deleteCommand = new Delete.Command(slug);

            var dbContext = GetDbContext();

            var handler = new Delete.Handler(dbContext);
            await handler.Handle(deleteCommand, new CancellationToken());

            var article = await ExecuteDbContextAsync(context =>
                context.Articles.Where(d => d.Slug == deleteCommand.Slug).SingleOrDefaultAsync());

            Assert.Null(article);
        }

        [Fact]
        public async Task Expect_Delete_Article_With_Tags()
        {
            var createCommand = new Create.Command
            {
                Article = new Create.ArticleData
                {
                    Title = "Test article 666",
                    Description = "Description of the test article",
                    Body = "Body of the test article",
                    TagList = new[] {"tag1", "tag2"}
                }
            };

            var createdArticle = await ArticleHelpers.CreateArticle(this, createCommand);
            var articleWithTags = await ExecuteDbContextAsync(context => context.Articles
                .Include(a => a.ArticleTags)
                .Where(d => d.Slug == createdArticle.Slug)
                .SingleOrDefaultAsync());

            var deleteCommand = new Delete.Command(createdArticle.Slug);

            var dbContext = GetDbContext();

            var handler = new Delete.Handler(dbContext);
            await handler.Handle(deleteCommand, new CancellationToken());

            var article = await ExecuteDbContextAsync(context =>
                context.Articles.Where(d => d.Slug == deleteCommand.Slug).SingleOrDefaultAsync());

            Assert.Null(article);
        }

        [Fact]
        public async Task Expect_Delete_Article_With_Comments()
        {
            var createArticleCommand = new Create.Command
            {
                Article = new Create.ArticleData
                {
                    Title = "Test article 666",
                    Description = "Description of the test article",
                    Body = "Body of the test article"
                }
            };

            var createdArticle = await ArticleHelpers.CreateArticle(this, createArticleCommand);
            var article = await ExecuteDbContextAsync(context => context.Articles
                .Include(a => a.ArticleTags)
                .Where(d => d.Slug == createdArticle.Slug)
                .SingleOrDefaultAsync());

            var articleId = article.ArticleId;
            var slug = article.Slug;

            var createCommentCommand = new beeforum.Features.Comments.Create.Command
            {
                Comment = new beeforum.Features.Comments.Create.CommentData
                {
                    Body = "article comment"
                },
                Slug = slug
            };

            var comment = await CommentHelpers.CreateComment(this, createCommentCommand, UserHelpers.DefaultUserName);

            // delete article with comment
            var deleteCommand = new Delete.Command(slug);

            var dbContext = GetDbContext();

            var handler = new Delete.Handler(dbContext);
            await handler.Handle(deleteCommand, new CancellationToken());

            var deleted = await ExecuteDbContextAsync(context =>
                context.Articles.Where(d => d.Slug == deleteCommand.Slug).SingleOrDefaultAsync());

            Assert.Null(deleted);
        }
    }
}