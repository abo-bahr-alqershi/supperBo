using MediatR;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.SponsoredAds;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.SponsoredAds
{
    public class RecordAdInteractionCommandHandler : IRequestHandler<RecordAdInteractionCommand, bool>
    {
        private readonly IRepository<SponsoredAdSection> _repository;

        public RecordAdInteractionCommandHandler(IRepository<SponsoredAdSection> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RecordAdInteractionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.AdId, cancellationToken);
            if (entity == null)
                return false;

            if (request.InteractionType == "impression")
                entity.RecordImpression();
            else if (request.InteractionType == "click")
                entity.RecordClick();
            else
                return false;

            await _repository.UpdateAsync(entity, cancellationToken);
            return true;
        }
    }
}