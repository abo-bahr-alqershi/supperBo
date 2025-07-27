using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Services;

/// <summary>
/// أمر لإنشاء خدمة جديدة للكيان
/// Command to create a new service for a property
/// </summary>
public class CreatePropertyServiceCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// اسم الخدمة
    /// Service name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// سعر الخدمة
    /// Service price
    /// </summary>
    public MoneyDto Price { get; set; }

    /// <summary>
    /// نموذج التسعير
    /// Pricing model
    /// </summary>
    public PricingModel PricingModel { get; set; }
} 