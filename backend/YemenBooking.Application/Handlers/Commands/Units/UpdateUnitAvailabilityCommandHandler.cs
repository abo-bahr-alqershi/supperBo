using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Units;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Units
{
    /// <summary>
    /// معالج أمر تحديث حالة توفر الوحدة
    /// </summary>
    public class UpdateUnitAvailabilityCommandHandler : IRequestHandler<UpdateUnitAvailabilityCommand, ResultDto<bool>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdateUnitAvailabilityCommandHandler> _logger;

        public UpdateUnitAvailabilityCommandHandler(
            IUnitRepository unitRepository,
            IPropertyRepository propertyRepository,
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdateUnitAvailabilityCommandHandler> logger)
        {
            _unitRepository = unitRepository;
            _propertyRepository = propertyRepository;
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdateUnitAvailabilityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث توفر الوحدة: UnitId={UnitId}, IsAvailable={IsAvailable}", request.UnitId, request.IsAvailable);

            // التحقق من المدخلات
            if (request.UnitId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الوحدة مطلوب");

            // التحقق من الوجود
            var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId, cancellationToken);
            if (unit == null)
                return ResultDto<bool>.Failed("الوحدة غير موجودة");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            var property = await _propertyRepository.GetPropertyByIdAsync(unit.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالوحدة غير موجود");
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث توفر هذه الوحدة");

            // التحقق من عدم وجود حجوزات مؤكدة عند جعل الوحدة غير متوفرة
            if (!request.IsAvailable)
            {
                bool hasActive = await _unitRepository.CheckActiveBookingsAsync(request.UnitId, cancellationToken);
                if (hasActive)
                    return ResultDto<bool>.Failed("لا يمكن جعل الوحدة غير متوفرة لوجود حجوزات مؤكدة أو مستقبلية");
            }

            // تنفيذ التحديث
            unit.IsAvailable = request.IsAvailable;
            unit.UpdatedBy = _currentUserService.UserId;
            unit.UpdatedAt = DateTime.UtcNow;
            await _unitRepository.UpdateUnitAsync(unit, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdateUnitAvailability",
                $"تم تحديث توفر الوحدة {request.UnitId} إلى {(request.IsAvailable ? "متوفرة" : "غير متوفرة")}",
                request.UnitId,
                "Unit",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث توفر الوحدة بنجاح: UnitId={UnitId}", request.UnitId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث توفر الوحدة بنجاح");
        }
    }
} 