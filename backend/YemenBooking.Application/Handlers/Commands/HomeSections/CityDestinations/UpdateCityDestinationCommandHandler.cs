using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.CityDestinations;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.CityDestinations
{
    public class UpdateCityDestinationCommandHandler : IRequestHandler<UpdateCityDestinationCommand, bool>
    {
        private readonly IRepository<CityDestinationSection> _repository;

        public UpdateCityDestinationCommandHandler(IRepository<CityDestinationSection> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCityDestinationCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) return false;

            entity.UpdateDestination(
                request.Name,
                request.NameAr,
                request.Country,
                request.CountryAr,
                request.Description,
                request.DescriptionAr,
                request.ImageUrl,
                System.Text.Json.JsonSerializer.Serialize(request.AdditionalImages),
                request.Latitude,
                request.Longitude,
                request.Currency,
                request.IsPopular,
                request.IsFeatured,
                request.Priority,
                System.Text.Json.JsonSerializer.Serialize(request.Highlights),
                System.Text.Json.JsonSerializer.Serialize(request.HighlightsAr),
                request.WeatherData,
                request.AttractionsData,
                request.Metadata);

            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}