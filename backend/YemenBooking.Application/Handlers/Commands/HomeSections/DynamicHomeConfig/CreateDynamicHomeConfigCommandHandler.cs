using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.Commands.HomeSections.DynamicHomeConfig;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.HomeSections.DynamicHomeConfig
{
    public class CreateDynamicHomeConfigCommandHandler : IRequestHandler<CreateDynamicHomeConfigCommand, Guid>
    {
        private readonly IRepository<DynamicHomeConfig> _repository;

        public CreateDynamicHomeConfigCommandHandler(IRepository<DynamicHomeConfig> repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateDynamicHomeConfigCommand request, CancellationToken cancellationToken)
        {
            var entity = new DynamicHomeConfig(
                request.Version,
                request.GlobalSettings,
                request.ThemeSettings,
                request.LayoutSettings,
                request.CacheSettings,
                request.AnalyticsSettings,
                request.EnabledFeatures,
                request.ExperimentalFeatures,
                request.Description);

            await _repository.AddAsync(entity, cancellationToken);
            return entity.Id;
        }
    }
}