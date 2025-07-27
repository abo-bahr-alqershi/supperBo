using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Policies;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Enums;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Interfaces;

namespace YemenBooking.Application.Handlers.Commands.Policies
{
    /// <summary>
    /// معالج أمر إنشاء سياسة جديدة للكيان
    /// </summary>
    public class CreatePropertyPolicyCommandHandler : IRequestHandler<CreatePropertyPolicyCommand, ResultDto<Guid>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<CreatePropertyPolicyCommandHandler> _logger;

        public CreatePropertyPolicyCommandHandler(
            IPropertyRepository propertyRepository,
            IPolicyRepository policyRepository,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<CreatePropertyPolicyCommandHandler> logger)
        {
            _propertyRepository = propertyRepository;
            _policyRepository = policyRepository;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<ResultDto<Guid>> Handle(CreatePropertyPolicyCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء إنشاء سياسة للكيان: PropertyId={PropertyId}, Type={Type}", request.PropertyId, request.PolicyType);

            // التحقق من صحة المدخلات
            if (request.PropertyId == Guid.Empty)
                return ResultDto<Guid>.Failed("معرف الكيان مطلوب");
            if (string.IsNullOrWhiteSpace(request.Description))
                return ResultDto<Guid>.Failed("وصف السياسة مطلوب");
            if (string.IsNullOrWhiteSpace(request.Rules))
                return ResultDto<Guid>.Failed("قواعد السياسة مطلوبة");

            // التحقق من وجود الكيان
            var property = await _propertyRepository.GetPropertyByIdAsync(request.PropertyId, cancellationToken);
            if (property == null)
                return ResultDto<Guid>.Failed("الكيان غير موجود");

            // التحقق من الصلاحيات (مالك الكيان أو مسؤول)
            if (_currentUserService.Role != "Admin" && property.OwnerId != _currentUserService.UserId)
                return ResultDto<Guid>.Failed("غير مصرح لك بإنشاء سياسة لهذا الكيان");

            // التحقق من عدم وجود سياسة من نفس النوع مسبقًا
            bool exists = await _policyRepository.ExistsAsync(p => p.PropertyId == request.PropertyId && p.Type == request.PolicyType, cancellationToken);
            if (exists)
                return ResultDto<Guid>.Failed("تمت إضافة سياسة من هذا النوع مسبقًا");

            // إنشاء كيان السياسة
            var policy = new PropertyPolicy
            {
                PropertyId = request.PropertyId,
                Type = request.PolicyType,
                Description = request.Description,
                Rules = request.Rules,
                CreatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow
            };
            var created = await _policyRepository.CreatePropertyPolicyAsync(policy, cancellationToken);

            // تسجيل العملية في سجل التدقيق
            await _auditService.LogBusinessOperationAsync(
                "CreatePropertyPolicy",
                $"تم إنشاء سياسة جديدة للكيان {request.PropertyId} من النوع {request.PolicyType}",
                created.Id,
                "PropertyPolicy",
                _currentUserService.UserId,
                new Dictionary<string, object> { { "PolicyId", created.Id }, { "Type", request.PolicyType } },
                cancellationToken);

            _logger.LogInformation("اكتمل إنشاء السياسة بنجاح: PolicyId={PolicyId}", created.Id);
            return ResultDto<Guid>.Succeeded(created.Id, "تم إنشاء السياسة بنجاح");
        }
    }
} 