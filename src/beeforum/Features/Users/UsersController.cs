using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace beeforum.Features.Users
{
    [Route("api/users")]
    public class UsersController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<UserEnvelope> Login([FromBody] Login.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost]
        public async Task<UserEnvelope> Create([FromBody] Create.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}