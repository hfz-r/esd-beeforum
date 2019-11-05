using System.Threading.Tasks;
using beeforum.Features.Users;

namespace beeforum.IntegrationTests.Features.Users
{
    public static class UserHelpers
    {
        public static readonly string DefaultUserName = "username";

        public static async Task<User> CreateDefaultUser(SliceFixture fixture)
        {
            var command = new Create.Command
            {
                User = new Create.UserData
                {
                    Email = "email",
                    Password = "password",
                    Username = DefaultUserName
                }
            };

            var commandResult = await fixture.SendAsync(command);

            return commandResult.User;
        }
    }
}