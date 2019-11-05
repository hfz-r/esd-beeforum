using System.Threading.Tasks;

namespace beeforum.Features.Profiles
{
    public interface IProfileReader
    {
        Task<ProfileEnvelope> ReadProfile(string username);
    }
}