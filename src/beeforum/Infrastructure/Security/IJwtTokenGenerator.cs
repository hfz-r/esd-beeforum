using System.Threading.Tasks;

namespace beeforum.Infrastructure.Security
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateToken(string username);
    }
}