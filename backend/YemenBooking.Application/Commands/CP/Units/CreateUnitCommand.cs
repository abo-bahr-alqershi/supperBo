using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

namespace YemenBooking.Application.Commands.Units;

/// <summary>
/// أمر لإنشاء وحدة جديدة في الكيان
/// Command to create a new unit in a property
/// </summary>
public class CreateUnitCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الكيان
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type ID
    /// </summary>
    public Guid UnitTypeId { get; set; }

    /// <summary>
    /// اسم الوحدة
    /// Unit name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// السعر الأساسي للوحدة
    /// Base price of the unit
    /// </summary>
    public MoneyDto BasePrice { get; set; }

    /// <summary>
    /// الميزات المخصصة للوحدة
    /// Custom features of the unit (JSON)
    /// </summary>
    public string CustomFeatures { get; set; } = string.Empty;

    /// <summary>
    /// طريقة حساب السعر
    /// Pricing calculation method (Hourly, Daily, Weekly, Monthly)
    /// </summary>
    public PricingMethod PricingMethod { get; set; }

    /// <summary>
    /// قيم الحقول الديناميكية للوحدة
    /// Dynamic field values for the unit
    /// </summary>
    public List<FieldValueDto> FieldValues { get; set; } = new List<FieldValueDto>();

    /// <summary>
    /// الصور المرسلة مؤقتاً للوحدة
    /// </summary>
    public List<string> Images { get; set; } = new List<string>();
} 