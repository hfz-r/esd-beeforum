using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Features.Comments;
using beeforum.IntegrationTests.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace beeforum.IntegrationTests.Features.Comments
{
    public static class CommentHelpers
    {
        public static async Task<Domain.Comment> CreateComment(SliceFixture fixture, Create.Command command, string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                var user = await UserHelpers.CreateDefaultUser(fixture);

                userName = user.Username;
            }

            var dbContext = fixture.GetDbContext();
            var currentUserAccessor = new StubCurrentUserAccessor(userName);

            var handler = new Create.Handler(dbContext, currentUserAccessor);
            var created = await handler.Handle(command, new CancellationToken());

            var articleWithComments = await fixture.ExecuteDbContextAsync(context =>
                context.Articles
                    .Include(a => a.Comments)
                    .Include(a => a.Author)
                    .Where(a => a.Slug == command.Slug)
                    .SingleOrDefaultAsync());

            var comment = articleWithComments.Comments
                .FirstOrDefault(c => c.ArticleId == articleWithComments.ArticleId &&
                                     c.Author == articleWithComments.Author);

            return comment;
        }
    }
}