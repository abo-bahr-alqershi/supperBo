using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.CityDestinations;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.CityDestinations
{
    public class CreateCityDestinationCommandHandler : IRequestHandler<CreateCityDestinationCommand, Guid>
    {
        private readonly IRepository<CityDestinationSection> _repository;

        public CreateCityDestinationCommandHandler(IRepository<CityDestinationSection> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateCityDestinationCommand request, CancellationToken cancellationToken)
        {
            var entity = new CityDestinationSection(
                request.Name,
                request.NameAr,
                request.Country,
                request.CountryAr,
                request.ImageUrl,
                request.Latitude,
                request.Longitude,
                request.Currency,
                request.Description,
                request.DescriptionAr,
                System.Text.Json.JsonSerializer.Serialize(request.AdditionalImages),
                0,
                0,
                0,
                0,
                request.IsPopular,
                request.IsFeatured,
                request.Priority,
                System.Text.Json.JsonSerializer.Serialize(request.Highlights),
                System.Text.Json.JsonSerializer.Serialize(request.HighlightsAr),
                request.WeatherData,
                request.AttractionsData,
                request.Metadata);

            await _repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
    }
}