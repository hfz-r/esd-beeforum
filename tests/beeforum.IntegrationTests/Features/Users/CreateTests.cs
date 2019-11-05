using System.Linq;
using System.Threading.Tasks;
using beeforum.Features.Users;
using beeforum.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace beeforum.IntegrationTests.Features.Users
{
    public class CreateTests : SliceFixture
    {
        [Fact]
        public async Task Expect_Create_User()
        {
            var command = new Create.Command
            {
                User = new Create.UserData
                {
                    Email = "email",
                    Password = "password",
                    Username = "username"
                }
            };

            await SendAsync(command);

            var created = await ExecuteDbContextAsync(context =>
                context.Persons.Where(p => p.Email == command.User.Email).SingleOrDefaultAsync());

            Assert.NotNull(created);
            Assert.Equal(created.Hash, new PasswordHasher().Hash("password", created.Salt));
        }
    }
}