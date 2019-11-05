using System.Net;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Articles
{
    public class Details
    {
        public class Query : IRequest<ArticleEnvelope>
        {
            public Query(string slug)
            {
                Slug = slug;
            }

            public string Slug { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, ArticleEnvelope>
        {
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<ArticleEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .GetAllData()
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);
                if (article == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NotFound});

                return new ArticleEnvelope(article);
            }
        }
    }
}