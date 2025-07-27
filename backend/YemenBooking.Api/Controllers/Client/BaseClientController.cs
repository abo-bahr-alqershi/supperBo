using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YemenBooking.Api.Controllers.Client
{
    [ApiController]
    [Route("api/client/[controller]")]
    [Authorize(Roles = "Client")]
    public abstract class BaseClientController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BaseClientController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
} 