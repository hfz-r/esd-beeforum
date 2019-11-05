using System.Net;
using System.Threading;
using System.Threading.Tasks;
using beeforum.Domain;
using beeforum.Features.Profiles;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Followers
{
    public class Add
    {
        public class Command : IRequest<ProfileEnvelope>
        {
            public Command(string username)
            {
                Username = username;
            }

            public string Username { get; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Username).NotNull().NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ProfileEnvelope>
        {
            private readonly ApplicationDbContext _context;
            private readonly ICurrentUserAccessor _currentUserAccessor;
            private readonly IProfileReader _profileReader;

            public Handler(ApplicationDbContext context, ICurrentUserAccessor currentUserAccessor, IProfileReader profileReader)
            {
                _context = context;
                _currentUserAccessor = currentUserAccessor;
                _profileReader = profileReader;
            }

            public async Task<ProfileEnvelope> Handle(Command message, CancellationToken cancellationToken)
            {
                var target = await _context.Persons.FirstOrDefaultAsync(x => x.Username == message.Username, cancellationToken);
                if (target == null)
                    throw new RestException(HttpStatusCode.NotFound, new {User = Constants.NotFound});

                var observer = await _context.Persons
                    .FirstOrDefaultAsync(x => x.Username == _currentUserAccessor.GetCurrentUsername(), cancellationToken);

                var followedPeople = await _context.FollowedPeople
                    .FirstOrDefaultAsync(x => x.ObserverId == observer.PersonId &&
                                              x.TargetId == target.PersonId, cancellationToken);

                if (followedPeople == null)
                {
                    followedPeople = new FollowedPeople
                    {
                        Observer = observer,
                        ObserverId = observer.PersonId,
                        Target = target,
                        TargetId = target.PersonId
                    };

                    await _context.FollowedPeople.AddAsync(followedPeople, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return await _profileReader.ReadProfile(message.Username);
            }
        }
    }
}