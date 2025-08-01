using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.DTOs;
using AutoMapper;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Queries
{
    public class GetActiveHomeScreenQuery : IRequest<ResultDto<HomeScreenTemplateDto>>
    {
        public string Platform { get; set; }
        public string TargetAudience { get; set; }
        public Guid? UserId { get; set; }
    }

    public class GetActiveHomeScreenQueryHandler : IRequestHandler<GetActiveHomeScreenQuery, ResultDto<HomeScreenTemplateDto>>
    {
        private readonly IHomeScreenRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetActiveHomeScreenQueryHandler(
            IHomeScreenRepository repository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ResultDto<HomeScreenTemplateDto>> Handle(GetActiveHomeScreenQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId ?? _currentUserService.UserId;
            
            // First check for user-specific customization
            if (userId.HasValue)
            {
                var userScreen = await _repository.GetUserHomeScreenAsync(userId.Value, request.Platform, cancellationToken);
                if (userScreen != null)
                {
                    var userTemplate = await _repository.GetTemplateWithFullHierarchyAsync(userScreen.TemplateId, cancellationToken);
                    var dto = _mapper.Map<HomeScreenTemplateDto>(userTemplate);
                    
                    // Apply user customizations
                    if (!string.IsNullOrEmpty(userScreen.CustomizationData))
                    {
                        dto.CustomizationData = userScreen.CustomizationData;
                        dto.UserPreferences = userScreen.UserPreferences;
                    }

                    return ResultDto<HomeScreenTemplateDto>.Ok(dto);
                }
            }

            // Get default active template
            var template = await _repository.GetActiveTemplateAsync(
                request.Platform,
                request.TargetAudience,
                cancellationToken);

            if (template == null)
            {
                // Get any default template as fallback
                template = await _repository.GetDefaultTemplateAsync(request.Platform, cancellationToken);
            }

            return ResultDto<HomeScreenTemplateDto>.Ok(_mapper.Map<HomeScreenTemplateDto>(template));
        }
    }
}