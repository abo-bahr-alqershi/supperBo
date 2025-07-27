using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Dashboard;
using YemenBooking.Application.DTOs.Dashboard;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Entities;
using Unit = MediatR.Unit;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Handlers.Commands.Dashboard
{
    /// <summary>
    /// معالج أمر التحديث الجماعي لتوفر الوحدات ضمن نطاق زمني
    /// </summary>
    public class BulkUpdateUnitAvailabilityCommandHandler : IRequestHandler<BulkUpdateUnitAvailabilityCommand, ResultDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAvailabilityService _availabilityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<BulkUpdateUnitAvailabilityCommandHandler> _logger;

        public BulkUpdateUnitAvailabilityCommandHandler(
            IUnitOfWork unitOfWork,
            IAvailabilityService availabilityService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<BulkUpdateUnitAvailabilityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _availabilityService = availabilityService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ResultDto<bool>> Handle(BulkUpdateUnitAvailabilityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء التحديث الجماعي لتوفر الوحدات: UnitCount={Count}, Range=[{StartDate}-{EndDate}], IsAvailable={IsAvailable}",
                request.UnitIds?.Count() ?? 0, request.Range.StartDate, request.Range.EndDate, request.IsAvailable);

            // التحقق من صحة المدخلات
            var errors = new List<string>();
            if (request.UnitIds == null || !request.UnitIds.Any())
                errors.Add("يجب تحديد قائمة وحدات للتحديث");
            if (request.Range == null)
                errors.Add("نطاق التواريخ مطلوب");
            else if (request.Range.StartDate >= request.Range.EndDate)
                errors.Add("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");

            if (errors.Any())
                throw new ArgumentException(string.Join("; ", errors));

            // التحقق من وجود الوحدات وصلاحيات المستخدم
            foreach (var unitId in request.UnitIds)
            {
                var unit = await _unitOfWork.Repository<YemenBooking.Core.Entities.Unit>().GetByIdAsync(unitId, cancellationToken);
                if (unit == null)
                    throw new KeyNotFoundException($"الوحدة غير موجودة: {unitId}");
                if (_currentUserService.Role != "Admin" && unit.Property.OwnerId != _currentUserService.UserId)
                    throw new UnauthorizedAccessException($"ليس لديك صلاحية تعديل توفر الوحدة: {unitId}");
            }

            // التنفيذ: حجز أو إلغاء حجز الفترات
            foreach (var unitId in request.UnitIds)
            {
                if (request.IsAvailable)
                {
                    await _availabilityService.UnblockUnitPeriodAsync(
                        unitId,
                        request.Range.StartDate,
                        request.Range.EndDate,
                        cancellationToken);
                }
                else
                {
                    await _availabilityService.BlockUnitPeriodAsync(
                        unitId,
                        request.Range.StartDate,
                        request.Range.EndDate,
                        "Bulk update unavailable",
                        cancellationToken);
                }

                // تسجيل التدقيق لكل وحدة
                await _auditService.LogActivityAsync(
                    "UnitAvailability",
                    unitId.ToString(),
                    "UPDATE",
                    $"تم تعيين توفر الوحدة إلى {(request.IsAvailable ? "متاح" : "غير متاح")} ضمن النطاق",
                    null,
                    new { request.Range.StartDate, request.Range.EndDate, request.IsAvailable },
                    cancellationToken);
            }

            _logger.LogInformation("اكتمل التحديث الجماعي لتوفر الوحدات بنجاح");
            return ResultDto<bool>.Ok(true, "تم تحديث توفر الوحدات بنجاح");
        }
    }
} 