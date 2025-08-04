using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Api.Controllers.Client;
using YemenBooking.Application.Queries.HomeSections;
using YemenBooking.Application.Commands.HomeSections.SponsoredAds;

namespace YemenBooking.Api.Controllers.Client
{
    public class HomeSectionsController : BaseClientController
    {
        public HomeSectionsController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Get dynamic home sections for mobile app
        /// </summary>
        [HttpGet("sections")]
        public async Task<ActionResult<List<DynamicHomeSectionDto>>> GetHomeSections([FromQuery] GetDynamicHomeSectionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get home configuration
        /// </summary>
        [HttpGet("config")]
        public async Task<ActionResult<DynamicHomeConfigDto>> GetHomeConfig([FromQuery] string version = null)
        {
            var q = new GetHomeConfigQuery { Version = version };
            var result = await _mediator.Send(q);
            if (result == null)
                return NotFound("No home configuration found");
            return Ok(result);
        }

        /// <summary>
        /// Get sponsored ads
        /// </summary>
        [HttpGet("sponsored-ads")]
        public async Task<ActionResult<List<SponsoredAdDto>>> GetSponsoredAds([FromQuery] GetSponsoredAdsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get city destinations
        /// </summary>
        [HttpGet("destinations")]
        public async Task<ActionResult<List<CityDestinationDto>>> GetCityDestinations([FromQuery] GetCityDestinationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Record ad impression
        /// </summary>
        [HttpPost("sponsored-ads/{adId}/impression")]
        public async Task<ActionResult> RecordAdImpression(
            Guid adId,
            [FromBody] AdInteractionRequest request = null)
        {
            var command = new RecordAdInteractionCommand(adId, "impression")
            {
                UserId = GetCurrentUserId(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = GetClientIpAddress(),
                AdditionalData = request?.AdditionalData ?? "{}"
            };

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound("Sponsored ad not found");
            return Ok();
        }

        /// <summary>
        /// Record ad click
        /// </summary>
        [HttpPost("sponsored-ads/{adId}/click")]
        public async Task<ActionResult> RecordAdClick(
            Guid adId,
            [FromBody] AdInteractionRequest request = null)
        {
            var command = new RecordAdInteractionCommand(adId, "click")
            {
                UserId = GetCurrentUserId(),
                UserAgent = Request.Headers["User-Agent"].ToString(),
                IpAddress = GetClientIpAddress(),
                AdditionalData = request?.AdditionalData ?? "{}"
            };

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound("Sponsored ad not found");
            return Ok();
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst("sub")?.Value ?? User?.FindFirst("id")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

    public class AdInteractionRequest
    {
        public string AdditionalData { get; set; } = "{}";
    }
}