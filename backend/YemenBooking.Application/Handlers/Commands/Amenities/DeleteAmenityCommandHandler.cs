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
    /// معالج أمر حذف المرفق
    /// </summary>
    public class DeleteAmenityCommandHandler : IRequestHandler<DeleteAmenityCommand, ResultDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DeleteAmenityCommandHandler> _logger;

        public DeleteAmenityCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<DeleteAmenityCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        /// <summary>
        /// تنفيذ منطق حذف المرفق
        /// </summary>
        public async Task<ResultDto<bool>> Handle(DeleteAmenityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء معالجة أمر حذف المرفق: {AmenityId}", request.AmenityId);

            try
            {
                // التحقق من صحة المدخلات
                var errors = new List<string>();
                if (request.AmenityId == Guid.Empty)
                    errors.Add("معرف المرفق مطلوب");

                if (errors.Count > 0)
                    return ResultDto<bool>.Failed(errors, "بيانات المدخلات غير صحيحة");

                // التحقق من وجود المرفق
                var existingAmenity = await _unitOfWork.Repository<Amenity>()
                    .GetByIdAsync(request.AmenityId, cancellationToken);
                if (existingAmenity == null)
                    return ResultDto<bool>.Failed("المرفق غير موجود");

                // التحقق من الصلاحيات
                if (_currentUserService.Role != "Admin")
                    return ResultDto<bool>.Failed("غير مصرح لك بحذف المرفق");

                // التحقق من عدم استخدام المرفق بأنواع الكيانات
                var isUsedInType = await _unitOfWork.Repository<PropertyTypeAmenity>()
                    .ExistsAsync(pta => pta.AmenityId == request.AmenityId, cancellationToken);
                if (isUsedInType)
                    return ResultDto<bool>.Failed("لا يمكن حذف المرفق لأنه مرتبط بأنواع الكيانات");

                // التنفيذ: الحذف الناعم
                existingAmenity.IsDeleted = true;
                existingAmenity.DeletedAt = DateTime.UtcNow;
                existingAmenity.DeletedBy = _currentUserService.UserId;

                await _unitOfWork.Repository<Amenity>()
                    .UpdateAsync(existingAmenity, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // الآثار الجانبية: تسجيل العملية في السجل
                await _auditService.LogActivityAsync(
                    nameof(Amenity),
                    existingAmenity.Id.ToString(),
                    "DELETE",
                    "تم حذف المرفق",
                    null,
                    existingAmenity,
                    cancellationToken);

                _logger.LogInformation("تم حذف المرفق بالمعرف {AmenityId}", existingAmenity.Id);
                return ResultDto<bool>.Succeeded(true, "تم حذف المرفق بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة أمر حذف المرفق: {AmenityId}", request.AmenityId);
                return ResultDto<bool>.Failed("حدث خطأ أثناء حذف المرفق");
            }
        }
    }
} 