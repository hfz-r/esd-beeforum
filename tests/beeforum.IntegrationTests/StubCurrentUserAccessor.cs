using beeforum.Infrastructure;

namespace beeforum.IntegrationTests
{
    public class StubCurrentUserAccessor : ICurrentUserAccessor
    {
        private readonly string _currentUserName;

        public StubCurrentUserAccessor(string currentUserName)
        {
            _currentUserName = currentUserName;
        }

        public string GetCurrentUsername()
        {
            return _currentUserName;
        }
    }
}