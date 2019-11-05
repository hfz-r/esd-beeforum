using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Comments
{
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(string slug, int id)
            {
                Slug = slug;
                Id = id;
            }

            public string Slug { get; }

            public int Id { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Slug).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly ApplicationDbContext _context;

            public Handler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var article = await _context.Articles
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);
                if (article == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NotFound});

                var comment = article.Comments.FirstOrDefault(x => x.CommentId == message.Id);
                if (comment == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Comment = Constants.NotFound});

                _context.Comments.Remove(comment);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}