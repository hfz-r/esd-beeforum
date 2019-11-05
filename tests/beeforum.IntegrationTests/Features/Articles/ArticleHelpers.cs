using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Features.Articles;
using beeforum.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace beeforum.IntegrationTests.Features.Articles
{
    public static class ArticleHelpers
    {
        public static async Task<Domain.Article> CreateArticle(SliceFixture fixture, Create.Command command)
        {
            var user = await UserHelpers.CreateDefaultUser(fixture);

            var dbContext = fixture.GetDbContext();
            var currentUserAccessor = new StubCurrentUserAccessor(user.Username);

            var handler = new Create.Handler(dbContext, currentUserAccessor);
            var created = await handler.Handle(command, new CancellationToken());

            var article = await fixture.ExecuteDbContextAsync(context =>
                context.Articles.Where(a => a.ArticleId == created.Article.ArticleId).SingleOrDefaultAsync());

            return article;
        }
    }
}