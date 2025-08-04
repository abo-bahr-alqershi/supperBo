using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.CityDestinations;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.CityDestinations
{
    public class UpdateCityDestinationStatsCommandHandler : IRequestHandler<UpdateCityDestinationStatsCommand, bool>
    {
        private readonly IRepository<CityDestinationSection> _repository;

        public UpdateCityDestinationStatsCommandHandler(IRepository<CityDestinationSection> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateCityDestinationStatsCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) return false;

            entity.UpdateStatistics(
                request.PropertyCount,
                request.AveragePrice,
                request.AverageRating,
                request.ReviewCount);

            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}