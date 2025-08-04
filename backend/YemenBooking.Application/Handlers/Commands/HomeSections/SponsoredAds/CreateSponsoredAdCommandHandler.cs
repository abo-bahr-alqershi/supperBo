using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using YemenBooking.Application.Commands.HomeSections.SponsoredAds;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.SponsoredAds
{
    public class CreateSponsoredAdCommandHandler : IRequestHandler<CreateSponsoredAdCommand, Guid>
    {
        private readonly IRepository<SponsoredAdSection> _repository;

        public CreateSponsoredAdCommandHandler(IRepository<SponsoredAdSection> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateSponsoredAdCommand request, CancellationToken cancellationToken)
        {
            var entity = new SponsoredAdSection(
                request.Title,
                JsonSerializer.Serialize(request.PropertyIds),
                request.CtaText,
                request.CtaAction,
                request.StartDate,
                request.EndDate,
                request.Priority,
                request.Subtitle,
                request.Description,
                request.CustomImageUrl,
                request.BackgroundColor,
                request.TextColor,
                request.Styling,
                request.CtaData,
                request.TargetingData,
                request.AnalyticsData);

            await _repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
    }
}