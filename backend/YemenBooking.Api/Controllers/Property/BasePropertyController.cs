using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YemenBooking.Api.Controllers.Property
{
    [ApiController]
    [Route("api/property/[controller]")]
    [Authorize(Roles = "Owner")]
    public abstract class BasePropertyController : ControllerBase
    {
        protected readonly IMediator _mediator;

        public BasePropertyController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
} 