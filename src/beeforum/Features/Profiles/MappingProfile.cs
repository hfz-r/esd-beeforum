using AutoMapper;

namespace beeforum.Features.Profiles
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Person, Profile>(MemberList.None);
        }
    }
}
