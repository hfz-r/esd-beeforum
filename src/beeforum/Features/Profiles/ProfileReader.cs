using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Data;
using beeforum.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace beeforum.Features.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserAccessor _currentUserAccessor;
        private readonly IMapper _mapper;

        public ProfileReader(ApplicationDbContext context, ICurrentUserAccessor currentUserAccessor, IMapper mapper)
        {
            _context = context;
            _currentUserAccessor = currentUserAccessor;
            _mapper = mapper;
        }

        public async Task<ProfileEnvelope> ReadProfile(string username)
        {
            var currentUserName = _currentUserAccessor.GetCurrentUsername();

            var person = await _context.Persons
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username);
            if (person == null)
                throw new RestException(HttpStatusCode.NotFound, new {User = Constants.NotFound});

            var profile = _mapper.Map<Domain.Person, Profile>(person);

            if (currentUserName != null)
            {
                var currentPerson = await _context.Persons
                    .Include(x => x.Following)
                    .Include(x => x.Followers)
                    .FirstOrDefaultAsync(x => x.Username == currentUserName);

                if (currentPerson.Followers.Any(x => x.TargetId == person.PersonId))
                {
                    profile.IsFollowed = true;
                }
            }

            return new ProfileEnvelope(profile);
        }
    }
}