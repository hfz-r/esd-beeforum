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
    public class Delete
    {
        public class Command : IRequest
        {
            public Command(string slug)
            {
                Slug = slug;
            }

            public string Slug { get; set; }
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
                var article = await _context.Articles.FirstOrDefaultAsync(x => x.Slug == message.Slug, cancellationToken);
                if (article == null)
                    throw new RestException(HttpStatusCode.NotFound, new {Article = Constants.NotFound});

                _context.Articles.Remove(article);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}