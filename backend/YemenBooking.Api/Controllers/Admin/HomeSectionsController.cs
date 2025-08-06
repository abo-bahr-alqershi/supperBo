using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Api.Controllers.Admin;
using YemenBooking.Application.Queries.HomeSections;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeSections;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig;
using YemenBooking.Application.Commands.HomeSections.CityDestinations;
using YemenBooking.Application.Commands.HomeSections.SponsoredAds;

namespace YemenBooking.Api.Controllers.Admin
{
    public class HomeSectionsController : BaseAdminController
    {
        public HomeSectionsController(IMediator mediator) : base(mediator) { }

        // Dynamic Home Sections
        [HttpGet("dynamic-sections")]
        public async Task<IActionResult> GetDynamicSections([FromQuery] GetDynamicHomeSectionsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("dynamic-sections")]
        public async Task<IActionResult> CreateDynamicSection([FromBody] CreateDynamicHomeSectionCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("dynamic-sections/{id}")]
        public async Task<IActionResult> UpdateDynamicSection(Guid id, [FromBody] UpdateDynamicHomeSectionCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("dynamic-sections/{id}/toggle")]
        public async Task<IActionResult> ToggleDynamicSection(Guid id, [FromQuery] bool? setActive = null)
        {
            var command = new ToggleSectionStatusCommand(id, setActive);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("dynamic-sections/{id}")]
        public async Task<IActionResult> DeleteDynamicSection(Guid id)
        {
            var command = new DeleteDynamicHomeSectionCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("dynamic-sections/reorder")]
        public async Task<IActionResult> ReorderDynamicSections([FromBody] ReorderDynamicHomeSectionsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Dynamic Home Config
        [HttpGet("dynamic-config")]
        public async Task<IActionResult> GetConfig([FromQuery] string version = null)
        {
            // Bind version manually to avoid model binding issues
            var query = new GetHomeConfigQuery { Version = version };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                var defaultConfig = new DynamicHomeConfigDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Version = "1.0",
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.ToString("O"),
                    UpdatedAt = DateTime.UtcNow.ToString("O"),
                    PublishedAt = null,
                    GlobalSettings = new Dictionary<string, object>(),
                    ThemeSettings = new Dictionary<string, object>(),
                    LayoutSettings = new Dictionary<string, object>(),
                    CacheSettings = new Dictionary<string, object>(),
                    AnalyticsSettings = new Dictionary<string, object>(),
                    EnabledFeatures = new List<string>(),
                    ExperimentalFeatures = new Dictionary<string, object>()
                };
                return Ok(defaultConfig);
            }
            return Ok(result);
        }

        [HttpPost("dynamic-config")]
        public async Task<IActionResult> CreateConfig([FromBody] CreateDynamicHomeConfigCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("dynamic-config/{id}")]
        public async Task<IActionResult> UpdateConfig(Guid id, [FromBody] UpdateDynamicHomeConfigCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("dynamic-config/{id}/publish")]
        public async Task<IActionResult> PublishConfig(Guid id)
        {
            var command = new PublishDynamicHomeConfigCommand(id);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // City Destinations
        [HttpGet("city-destinations")]
        public async Task<IActionResult> GetCityDestinations([FromQuery] GetCityDestinationsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("city-destinations")]
        public async Task<IActionResult> CreateCityDestination([FromBody] CreateCityDestinationCommand request)
        {
            var id = await _mediator.Send(request);
            return Ok(id);
        }

        [HttpPut("city-destinations/{id}")]
        public async Task<IActionResult> UpdateCityDestination(Guid id, [FromBody] UpdateCityDestinationCommand request)
        {
            request.Id = id;
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPut("city-destinations/{id}/stats")]
        public async Task<IActionResult> UpdateCityDestinationStats(Guid id, [FromBody] CityDestinationStatsDto stats)
        {
            var command = new UpdateCityDestinationStatsCommand(
                id,
                stats.PropertyCount,
                stats.AveragePrice,
                stats.AverageRating,
                stats.ReviewCount);
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // Sponsored Ads
        [HttpGet("sponsored-ads")]
        public async Task<IActionResult> GetSponsoredAds([FromQuery] GetSponsoredAdsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("sponsored-ads")]
        public async Task<IActionResult> CreateSponsoredAd([FromBody] CreateSponsoredAdCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPut("sponsored-ads/{id}")]
        public async Task<IActionResult> UpdateSponsoredAd(Guid id, [FromBody] UpdateSponsoredAdCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }

    public class CityDestinationStatsDto
    {
        public int PropertyCount { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}