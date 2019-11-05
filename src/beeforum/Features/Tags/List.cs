using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Tags
{
    public class List
    {
        public class Query : IRequest<TagsEnvelope>
        {
        }

        public class QueryHandler : IRequestHandler<Query, TagsEnvelope>
        {
            private readonly ApplicationDbContext _context;

            public QueryHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<TagsEnvelope> Handle(Query message, CancellationToken cancellationToken)
            {
                var tags = await _context.Tags
                    .OrderBy(x => x.TagId)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return new TagsEnvelope()
                {
                    Tags = tags.Select(x => x.TagId).ToList()
                };
            }
        }
    }
}