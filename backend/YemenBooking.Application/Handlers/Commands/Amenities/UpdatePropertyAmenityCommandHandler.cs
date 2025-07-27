using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Amenities;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Entities;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.Commands.Amenities
{
    /// <summary>
    /// معالج أمر تحديث إعدادات المرفق في الكيان
    /// </summary>
    public class UpdatePropertyAmenityCommandHandler : IRequestHandler<UpdatePropertyAmenityCommand, ResultDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdatePropertyAmenityCommandHandler> _logger;

        public UpdatePropertyAmenityCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdatePropertyAmenityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// تنفيذ منطق تحديث إعدادات المرفق في الكيان
        /// </summary>
        public async Task<ResultDto<bool>> Handle(UpdatePropertyAmenityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر تحديث إعدادات المرفق في الكيان: PropertyId={PropertyId}, AmenityId={AmenityId}", request.PropertyId, request.AmenityId);

            try
            {
                // التحقق من صحة المدخلات
                var errors = new List<string>();
                if (request.PropertyId == Guid.Empty)
                    errors.Add("معرف الكيان مطلوب");
                if (request.AmenityId == Guid.Empty)
                    errors.Add("معرف المرفق مطلوب");
                if (request.ExtraCost == null)
                    errors.Add("التكلفة الإضافية مطلوبة");

                if (errors.Count > 0)
                    return ResultDto<bool>.Failed(errors, "بيانات المدخلات غير صحيحة");

                // التحقق من وجود الكيان والمرفق
                var property = await _unitOfWork.Repository<Property>()
                    .GetByIdAsync(request.PropertyId, cancellationToken);
                if (property == null)
                    return ResultDto<bool>.Failed("الكيان غير موجود");

                // العثور على تخصيص نوع الكيان والمرفق
                var pta = await _unitOfWork.Repository<PropertyTypeAmenity>()
                    .FirstOrDefaultAsync(x => x.PropertyTypeId == property.Id && x.AmenityId == request.AmenityId, cancellationToken);
                if (pta == null)
                    return ResultDto<bool>.Failed("المرفق غير مخصص لنوع الكيان");

                // العثور على علاقة المرفق بالكيان
                var propertyAmenity = await _unitOfWork.Repository<PropertyAmenity>()
                    .FirstOrDefaultAsync(x => x.PropertyId == request.PropertyId && x.PtaId == pta.Id, cancellationToken);
                if (propertyAmenity == null)
                    return ResultDto<bool>.Failed("المرفق غير مرتبط بهذا الكيان");

                // التحقق من الصلاحيات
                if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                    return ResultDto<bool>.Failed("ليس لديك صلاحية لتحديث إعدادات المرفق في هذا الكيان");

                // حفظ القيم القديمة للتدقيق
                var oldValues = new PropertyAmenity
                {
                    Id = propertyAmenity.Id,
                    PropertyId = propertyAmenity.PropertyId,
                    PtaId = propertyAmenity.PtaId,
                    IsAvailable = propertyAmenity.IsAvailable,
                    ExtraCost = propertyAmenity.ExtraCost,
                    CreatedAt = propertyAmenity.CreatedAt,
                    UpdatedAt = propertyAmenity.UpdatedAt,
                    IsDeleted = propertyAmenity.IsDeleted,
                    DeletedAt = propertyAmenity.DeletedAt
                };

                // التنفيذ: تحديث القيم
                propertyAmenity.IsAvailable = request.IsAvailable;
                propertyAmenity.ExtraCost = new Money(request.ExtraCost.Amount, request.ExtraCost.Currency);
                propertyAmenity.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<PropertyAmenity>().UpdateAsync(propertyAmenity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // الآثار الجانبية: تسجيل العملية في السجل
                await _auditService.LogActivityAsync(
                    nameof(PropertyAmenity),
                    propertyAmenity.Id.ToString(),
                    "UPDATE",
                    "تم تحديث إعدادات المرفق في الكيان",
                    oldValues,
                    propertyAmenity,
                    cancellationToken);

                _logger.LogInformation("تم تحديث إعدادات المرفق في الكيان: PropertyAmenityId={PaId}", propertyAmenity.Id);
                return ResultDto<bool>.Succeeded(true, "تم تحديث إعدادات المرفق في الكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة أمر تحديث إعدادات المرفق في الكيان: PropertyId={PropertyId}, AmenityId={AmenityId}", request.PropertyId, request.AmenityId);
                return ResultDto<bool>.Failed("حدث خطأ أثناء تحديث إعدادات المرفق في الكيان");
            }
        }
    }
} 