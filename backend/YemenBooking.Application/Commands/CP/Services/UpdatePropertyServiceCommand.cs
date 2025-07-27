using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Services;

/// <summary>
/// أمر لتحديث بيانات الخدمة
/// Command to update service information
/// </summary>
public class UpdatePropertyServiceCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الخدمة
    /// Service ID
    /// </summary>
    public Guid ServiceId { get; set; }

    /// <summary>
    /// اسم الخدمة المحدث
    /// Updated service name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// سعر الخدمة المحدث
    /// Updated service price
    /// </summary>
    public MoneyDto? Price { get; set; }

    /// <summary>
    /// نموذج التسعير المحدث
    /// Updated pricing model
    /// </summary>
    public PricingModel? PricingModel { get; set; }
}