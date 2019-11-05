using System.Threading.Tasks;
using beeforum.Infrastructure;
using beeforum.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace beeforum.Features.Users
{
    [Route("api/user")]
    [Authorize(AuthenticationSchemes = JwtIssuerOptions.Schemes)]
    public class UserController
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserAccessor _currentUserAccessor;

        public UserController(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
        {
            _mediator = mediator;
            _currentUserAccessor = currentUserAccessor;
        }

        [HttpGet]
        public async Task<UserEnvelope> GetCurrent()
        {
            return await _mediator.Send(new Details.Query()
            {
                Username = _currentUserAccessor.GetCurrentUsername()
            });
        }

        [HttpPut]
        public async Task<UserEnvelope> UpdateUser([FromBody] Edit.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}