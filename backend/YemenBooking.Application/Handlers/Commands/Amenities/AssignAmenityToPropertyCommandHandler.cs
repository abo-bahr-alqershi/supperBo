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
    /// معالج أمر ربط المرفق بكيان
    /// </summary>
    public class AssignAmenityToPropertyCommandHandler : IRequestHandler<AssignAmenityToPropertyCommand, ResultDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<AssignAmenityToPropertyCommandHandler> _logger;

        public AssignAmenityToPropertyCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<AssignAmenityToPropertyCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// تنفيذ منطق ربط المرفق بكيان
        /// </summary>
        public async Task<ResultDto<bool>> Handle(AssignAmenityToPropertyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر ربط المرفق بالكيان: PropertyId={PropertyId}, AmenityId={AmenityId}", request.PropertyId, request.AmenityId);

            try
            {
                // التحقق من صحة المدخلات
                var errors = new List<string>();
                if (request.PropertyId == Guid.Empty)
                    errors.Add("معرف الكيان مطلوب");
                if (request.AmenityId == Guid.Empty)
                    errors.Add("معرف المرفق مطلوب");

                if (errors.Count > 0)
                    return ResultDto<bool>.Failed(errors, "بيانات المدخلات غير صحيحة");

                // التحقق من وجود الكيان والمرفق
                var property = await _unitOfWork.Repository<Property>()
                    .GetByIdAsync(request.PropertyId, cancellationToken);
                if (property == null)
                    return ResultDto<bool>.Failed("الكيان غير موجود");

                var amenity = await _unitOfWork.Repository<Amenity>()
                    .GetByIdAsync(request.AmenityId, cancellationToken);
                if (amenity == null)
                    return ResultDto<bool>.Failed("المرفق غير موجود");

                // التحقق من الصلاحيات
                if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                    return ResultDto<bool>.Failed("ليس لديك صلاحية لربط المرفق بهذا الكيان");

                // التحقق من وجود علاقة نوع الكيان والمرفق
                var pta = await _unitOfWork.Repository<PropertyTypeAmenity>()
                    .FirstOrDefaultAsync(x => x.PropertyTypeId == property.Id && x.AmenityId == request.AmenityId, cancellationToken);
                if (pta == null)
                    return ResultDto<bool>.Failed("المرفق غير مخصص لنوع الكيان");

                // التحقق من عدم وجود الربط مسبقاً
                var exists = await _unitOfWork.Repository<PropertyAmenity>()
                    .ExistsAsync(x => x.PropertyId == request.PropertyId && x.PtaId == pta.Id, cancellationToken);
                if (exists)
                    return ResultDto<bool>.Failed("المرفق مرتبط مسبقاً بهذا الكيان");

                // التنفيذ: إنشاء ربط
                var propertyAmenity = new PropertyAmenity
                {
                    PropertyId = request.PropertyId,
                    PtaId = pta.Id,
                    IsAvailable = true,
                    ExtraCost = Money.Zero("YER")
                };

                await _unitOfWork.Repository<PropertyAmenity>().AddAsync(propertyAmenity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // الآثار الجانبية: تسجيل العملية في السجل
                await _auditService.LogActivityAsync(
                    nameof(PropertyAmenity),
                    propertyAmenity.Id.ToString(),
                    "CREATE",
                    "تم ربط المرفق بالكيان بنجاح",
                    null,
                    propertyAmenity,
                    cancellationToken);

                _logger.LogInformation("تم ربط المرفق بالكيان: PropertyAmenityId={PaId}", propertyAmenity.Id);
                return ResultDto<bool>.Succeeded(true, "تم ربط المرفق بالكيان بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة أمر ربط المرفق بالكيان: PropertyId={PropertyId}, AmenityId={AmenityId}", request.PropertyId, request.AmenityId);
                return ResultDto<bool>.Failed("حدث خطأ أثناء ربط المرفق بالكيان");
            }
        }
    }
} 