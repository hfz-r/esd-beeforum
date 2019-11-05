using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Articles
{
    public class List
    {
        public class Query : IRequest<ArticlesEnvelope>
        {
            public Query(string tag, string author, string favorited, int? limit, int? offset)
            {
                Tag = tag;
                Author = author;
                FavoritedUsername = favorited;
                Limit = limit;
                Offset = offset;
            }

            public string Tag { get; }

            public string Author { get; }

            public string FavoritedUsername { get; }

            public int? Limit { get; }

            public int? Offset { get; }

            public bool IsFeed { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, ArticlesEnvelope>
        {
            private readonly ApplicationDbContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;

            public QueryHandler(ApplicationDbContext context, ICurrentUserAccessor currentUserAccessor)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
            }

            public async Task<ArticlesEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var query = _context.Articles.GetAllData();

                if (message.IsFeed && _currentUserAccessor.GetCurrentUsername() != null)
                {
                    var currentUser = await _context.Persons
                        .Include(x => x.Following)
                        .FirstOrDefaultAsync(x => x.Username == _currentUserAccessor.GetCurrentUsername(), cancellationToken);

                    query = query.Where(x => currentUser.Following.Select(y => y.TargetId)
                        .Contains(x.Author.PersonId));
                }

                if (!string.IsNullOrWhiteSpace(message.Tag))
                {
                    var tag = await _context.ArticleTags.FirstOrDefaultAsync(x => x.TagId == message.Tag, cancellationToken);

                    if (tag != null)
                        query = query.Where(x => x.ArticleTags.Select(y => y.TagId).Contains(tag.TagId));
                    else
                        return new ArticlesEnvelope();
                }

                if (!string.IsNullOrWhiteSpace(message.Author))
                {
                    var author = await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.Author, cancellationToken);

                    if (author != null)
                        query = query.Where(x => x.Author == author);
                    else
                        return new ArticlesEnvelope();
                }

                if (!string.IsNullOrWhiteSpace(message.FavoritedUsername))
                {
                    var author = await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.FavoritedUsername, cancellationToken);

                    if (author != null)
                        query = query.Where(x => x.ArticleFavorites.Any(y => y.PersonId == author.PersonId));
                    else
                        return new ArticlesEnvelope();
                }

                var articles = await query
                    .OrderByDescending(x => x.CreatedAt)
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new ArticlesEnvelope
                {
                    Articles = articles,
                    ArticlesCount = query.Count()
                };
            }
        }
    }
}