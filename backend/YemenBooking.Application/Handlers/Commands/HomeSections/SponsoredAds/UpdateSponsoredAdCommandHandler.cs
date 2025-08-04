using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using YemenBooking.Application.Commands.HomeSections.SponsoredAds;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.SponsoredAds
{
    public class UpdateSponsoredAdCommandHandler : IRequestHandler<UpdateSponsoredAdCommand, bool>
    {
        private readonly IRepository<SponsoredAdSection> _repository;

        public UpdateSponsoredAdCommandHandler(IRepository<SponsoredAdSection> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateSponsoredAdCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null) return false;

            entity.UpdateAd(
                request.Title,
                request.Subtitle,
                request.Description,
                JsonSerializer.Serialize(request.PropertyIds),
                request.CustomImageUrl,
                request.BackgroundColor,
                request.TextColor,
                request.Styling,
                request.CtaText,
                request.CtaAction,
                request.CtaData,
                request.StartDate,
                request.EndDate,
                request.Priority,
                request.TargetingData,
                request.AnalyticsData);

            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}