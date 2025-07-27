using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.Units;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Application.Handlers.Queries.Units
{
    /// <summary>
    /// معالج استعلام جلب بيانات الوحدة للتحرير
    /// Query handler for GetUnitForEditQuery
    /// </summary>
    public class GetUnitForEditQueryHandler : IRequestHandler<GetUnitForEditQuery, ResultDto<UnitEditDto>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetUnitForEditQueryHandler> _logger;

        public GetUnitForEditQueryHandler(
            IUnitRepository unitRepository,
            ICurrentUserService currentUserService,
            ILogger<GetUnitForEditQueryHandler> logger)
        {
            _unitRepository = unitRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<ResultDto<UnitEditDto>> Handle(GetUnitForEditQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام بيانات الوحدة للتحرير: {UnitId}", request.UnitId);

            var unit = await _unitRepository.GetQueryable()
                .AsNoTracking()
                .Include(u => u.FieldValues)
                    .ThenInclude(fv => fv.UnitTypeField)
                .FirstOrDefaultAsync(u => u.Id == request.UnitId, cancellationToken);

            if (unit == null)
                return ResultDto<UnitEditDto>.Failure($"الوحدة بالمعرف {request.UnitId} غير موجود");

            var currentUser = await _currentUserService.GetCurrentUserAsync(cancellationToken);
            var role = _currentUserService.Role;
            bool isOwner = currentUser != null && _currentUserService.UserId == request.OwnerId;
            if (role != "Admin" && !isOwner)
                return ResultDto<UnitEditDto>.Failure("ليس لديك صلاحية لتحرير هذه الوحدة");

            // تم تعطيل تجميع الحقول الديناميكية مؤقتاً لتجنب أخطاء التوثيق
            var dynamicFields = new List<FieldGroupWithValuesDto>();

            var customFeatures = JsonSerializer.Deserialize<Dictionary<string, object>>(unit.CustomFeatures)
                ?? new Dictionary<string, object>();

            var dto = new UnitEditDto
            {
                UnitId = unit.Id,
                Name = unit.Name,
                BasePrice = unit.BasePrice.Amount,
                CustomFeatures = customFeatures,
                DynamicFields = dynamicFields
            };

            _logger.LogInformation("تم جلب بيانات الوحدة للتحرير بنجاح: {UnitId}", request.UnitId);
            return ResultDto<UnitEditDto>.Succeeded(dto);
        }
    }
} 