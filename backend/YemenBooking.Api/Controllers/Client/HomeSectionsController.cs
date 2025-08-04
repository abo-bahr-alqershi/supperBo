using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YemenBooking.Application.Queries.MobileApp.HomeSections;
using YemenBooking.Application.Commands.SponsoredAds;
using System;

namespace YemenBooking.Api.Controllers.Client
{
    [ApiController]
    [Route("api/client/[controller]")]
    public class HomeSectionsController : BaseClientController
    {
        public HomeSectionsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Get dynamic home sections for mobile app
        /// </summary>
        [HttpGet("sections")]
        public async Task<ActionResult<List<DynamicHomeSectionDto>>> GetHomeSections(
            [FromQuery] string language = "en",
            [FromQuery] string[] targetAudience = null,
            [FromQuery] bool includeContent = true,
            [FromQuery] bool onlyActive = true)
        {
            var query = new GetDynamicHomeSectionsQuery
            {
                Language = language,
                TargetAudience = targetAudience?.ToList() ?? new List<string>(),
                IncludeContent = includeContent,
                OnlyActive = onlyActive
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get home configuration
        /// </summary>
        [HttpGet("config")]
        public async Task<ActionResult<DynamicHomeConfigDto>> GetHomeConfig(
            [FromQuery] string version = null)
        {
            var query = new GetHomeConfigQuery
            {
                Version = version
            };

            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound("No home configuration found");
            }

            return Ok(result);
        }

        /// <summary>
        /// Get sponsored ads
        /// </summary>
        [HttpGet("sponsored-ads")]
        public async Task<ActionResult<List<SponsoredAdDto>>> GetSponsoredAds(
            [FromQuery] bool onlyActive = true,
            [FromQuery] string[] targetAudience = null,
            [FromQuery] int? limit = null,
            [FromQuery] bool includePropertyDetails = false)
        {
            var query = new GetSponsoredAdsQuery
            {
                OnlyActive = onlyActive,
                TargetAudience = targetAudience?.ToList() ?? new List<string>(),
                Limit = limit,
                IncludePropertyDetails = includePropertyDetails
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get city destinations
        /// </summary>
        [HttpGet("destinations")]
        public async Task<ActionResult<List<CityDestinationDto>>> GetCityDestinations(
            [FromQuery] string language = "en",
            [FromQuery] bool onlyActive = true,
            [FromQuery] bool onlyPopular = false,
            [FromQuery] bool onlyFeatured = false,
            [FromQuery] int? limit = null,
            [FromQuery] string sortBy = "priority")
        {
            var query = new GetCityDestinationsQuery
            {
                Language = language,
                OnlyActive = onlyActive,
                OnlyPopular = onlyPopular,
                OnlyFeatured = onlyFeatured,
                Limit = limit,
                SortBy = sortBy
            };

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
            {
                return NotFound("Sponsored ad not found");
            }

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
            {
                return NotFound("Sponsored ad not found");
            }

            return Ok();
        }

        private Guid? GetCurrentUserId()
        {
            // Implementation depends on your authentication system
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