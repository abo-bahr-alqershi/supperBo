using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Exceptions;
using YemenBooking.Application.Queries.Dashboard;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Handlers.Queries.Dashboard
{
    /// <summary>
    /// معالج استعلام إحصائيات صور الكيان
    /// Handler for GetPropertyImageStatsQuery
    /// </summary>
    public class GetPropertyImageStatsQueryHandler : IRequestHandler<GetPropertyImageStatsQuery, ResultDto<PropertyImageStatsDto>>
    {
        private readonly IPropertyImageRepository _imageRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetPropertyImageStatsQueryHandler> _logger;

        public GetPropertyImageStatsQueryHandler(
            IPropertyImageRepository imageRepository,
            ICurrentUserService currentUserService,
            ILogger<GetPropertyImageStatsQueryHandler> logger)
        {
            _imageRepository = imageRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<PropertyImageStatsDto>> Handle(GetPropertyImageStatsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Property Image Stats Query for Property {PropertyId}", request.PropertyId);

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            if (currentUser == null)
                return ResultDto<PropertyImageStatsDto>.Failure("يجب تسجيل الدخول لعرض إحصائيات الصور");

            var role = _currentUserService.Role;
            if (role != "Admin" && _currentUserService.PropertyId != request.PropertyId)
                return ResultDto<PropertyImageStatsDto>.Failure("ليس لديك صلاحية لعرض إحصائيات صور هذا الكيان");

            var baseQuery = _imageRepository.GetQueryable()
                .Where(pi => pi.PropertyId == request.PropertyId && !pi.IsDeleted);

            var totalImages = await baseQuery.CountAsync(cancellationToken);
            var pendingCount = await baseQuery.Where(pi => pi.Status == ImageStatus.Pending).CountAsync(cancellationToken);
            var approvedCount = await baseQuery.Where(pi => pi.Status == ImageStatus.Approved).CountAsync(cancellationToken);
            var rejectedCount = await baseQuery.Where(pi => pi.Status == ImageStatus.Rejected).CountAsync(cancellationToken);

            var dto = new PropertyImageStatsDto
            {
                PropertyId = request.PropertyId,
                TotalImages = totalImages,
                PendingCount = pendingCount,
                ApprovedCount = approvedCount,
                RejectedCount = rejectedCount
            };

            return ResultDto<PropertyImageStatsDto>.Ok(dto);
        }
    }
} 