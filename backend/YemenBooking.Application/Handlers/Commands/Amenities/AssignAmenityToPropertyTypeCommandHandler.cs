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

namespace YemenBooking.Application.Handlers.Commands.Amenities
{
    /// <summary>
    /// معالج أمر تخصيص المرفق لنوع الكيان
    /// </summary>
    public class AssignAmenityToPropertyTypeCommandHandler : IRequestHandler<AssignAmenityToPropertyTypeCommand, ResultDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AssignAmenityToPropertyTypeCommandHandler> _logger;

        public AssignAmenityToPropertyTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<AssignAmenityToPropertyTypeCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// تنفيذ منطق تخصيص المرفق لنوع الكيان
        /// </summary>
        public async Task<ResultDto<bool>> Handle(AssignAmenityToPropertyTypeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر تخصيص المرفق لنوع الكيان: PropertyTypeId={PropertyTypeId}, AmenityId={AmenityId}", request.PropertyTypeId, request.AmenityId);

            try
            {
                // التحقق من صحة المدخلات
                var errors = new List<string>();
                if (request.PropertyTypeId == Guid.Empty)
                    errors.Add("معرف نوع الكيان مطلوب");
                if (request.AmenityId == Guid.Empty)
                    errors.Add("معرف المرفق مطلوب");

                if (errors.Count > 0)
                    return ResultDto<bool>.Failed(errors, "بيانات المدخلات غير صحيحة");

                // التحقق من وجود نوع الكيان والمرفق
                var propertyType = await _unitOfWork.Repository<PropertyType>()
                    .GetByIdAsync(request.PropertyTypeId, cancellationToken);
                if (propertyType == null)
                    return ResultDto<bool>.Failed("نوع الكيان غير موجود");

                var amenity = await _unitOfWork.Repository<Amenity>()
                    .GetByIdAsync(request.AmenityId, cancellationToken);
                if (amenity == null)
                    return ResultDto<bool>.Failed("المرفق غير موجود");

                // التحقق من الصلاحيات
                if (_currentUserService.Role != "Admin")
                    return ResultDto<bool>.Failed("غير مصرح لك بتخصيص المرفق لنوع الكيان");

                // التحقق من عدم وجود التخصيص مسبقاً
                var exists = await _unitOfWork.Repository<PropertyTypeAmenity>()
                    .ExistsAsync(x => x.PropertyTypeId == request.PropertyTypeId && x.AmenityId == request.AmenityId, cancellationToken);
                if (exists)
                    return ResultDto<bool>.Failed("المرفق مخصص مسبقاً لنوع الكيان");

                // التنفيذ: إنشاء التخصيص
                var pta = new PropertyTypeAmenity
                {
                    PropertyTypeId = request.PropertyTypeId,
                    AmenityId = request.AmenityId,
                    IsDefault = request.IsDefault
                };

                await _unitOfWork.Repository<PropertyTypeAmenity>().AddAsync(pta, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // الآثار الجانبية: تسجيل العملية في السجل
                await _auditService.LogActivityAsync(
                    nameof(PropertyTypeAmenity),
                    pta.Id.ToString(),
                    "CREATE",
                    "تم تخصيص المرفق لنوع الكيان بنجاح",
                    null,
                    pta,
                    cancellationToken);

                _logger.LogInformation("تم تخصيص المرفق لنوع الكيان: PtaId={PtaId}", pta.Id);
                return ResultDto<bool>.Succeeded(true, "تم تخصيص المرفق لنوع الكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة أمر تخصيص المرفق لنوع الكيان: PropertyTypeId={PropertyTypeId}, AmenityId={AmenityId}", request.PropertyTypeId, request.AmenityId);
                return ResultDto<bool>.Failed("حدث خطأ أثناء تخصيص المرفق لنوع الكيان");
            }
        }
    }
} 