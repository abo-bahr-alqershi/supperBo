using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Services;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.ValueObjects;

namespace YemenBooking.Application.Handlers.Commands.Services
{
    /// <summary>
    /// معالج أمر تحديث بيانات خدمة الكيان
    /// </summary>
    public class UpdatePropertyServiceCommandHandler : IRequestHandler<UpdatePropertyServiceCommand, ResultDto<bool>>
    {
        private readonly IPropertyServiceRepository _serviceRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UpdatePropertyServiceCommandHandler> _logger;

        public UpdatePropertyServiceCommandHandler(
            IPropertyServiceRepository serviceRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UpdatePropertyServiceCommandHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<bool>> Handle(UpdatePropertyServiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء تحديث خدمة الكيان: ServiceId={ServiceId}", request.ServiceId);

            // التحقق من المدخلات
            if (request.ServiceId == Guid.Empty)
                return ResultDto<bool>.Failed("معرف الخدمة مطلوب");
            if (request.Name != null && string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<bool>.Failed("اسم الخدمة المطلوب غير صالح");
            if (request.Price != null && request.Price.Amount <= 0)
                return ResultDto<bool>.Failed("السعر يجب أن يكون أكبر من صفر");

            // التحقق من الوجود
            var service = await _serviceRepository.GetPropertyServiceByIdAsync(request.ServiceId, cancellationToken);
            if (service == null)
                return ResultDto<bool>.Failed("الخدمة غير موجودة");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            var property = await _serviceRepository.GetPropertyByIdAsync(service.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<bool>.Failed("الكيان المرتبط بالخدمة غير موجود");
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<bool>.Failed("غير مصرح لك بتحديث هذه الخدمة");

            // التحقق من التكرار عند تغيير الاسم
            if (!string.IsNullOrWhiteSpace(request.Name) && !string.Equals(service.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool duplicate = await _serviceRepository.ExistsAsync(s => s.PropertyId == service.PropertyId && s.Name.Trim() == request.Name.Trim() && s.Id != request.ServiceId, cancellationToken);
                if (duplicate)
                    return ResultDto<bool>.Failed("يوجد خدمة أخرى بنفس الاسم لهذا الكيان");
                service.Name = request.Name.Trim();
            }

            // apply updates
            if (request.Price != null)
                service.Price = new Money(request.Price.Amount, request.Price.Currency);
            if (request.PricingModel.HasValue)
                service.PricingModel = request.PricingModel.Value;

            service.UpdatedBy = _currentUserService.UserId;
            service.UpdatedAt = DateTime.UtcNow;

            await _serviceRepository.UpdatePropertyServiceAsync(service, cancellationToken);

            // تسجيل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "UpdatePropertyService",
                $"تم تحديث الخدمة {request.ServiceId}",
                request.ServiceId,
                "PropertyService",
                _currentUserService.UserId,
                null,
                cancellationToken);

            _logger.LogInformation("اكتمل تحديث الخدمة بنجاح: ServiceId={ServiceId}", request.ServiceId);
            return ResultDto<bool>.Succeeded(true, "تم تحديث الخدمة بنجاح");
        }
    }
} 