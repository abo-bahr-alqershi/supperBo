namespace YemenBooking.Application.Commands.Units;

using System;
using System.Collections.Generic;
using MediatR;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Enums;

/// <summary>
/// إنشاء وحدة جديدة مع قيم الحقول الديناميكية
/// Create a new unit along with dynamic field values
/// </summary>
public class CreateUnitWithFieldValuesCommand : IRequest<ResultDto<Guid>>
{
    /// <summary>
    /// معرف الكيان
    /// Property identifier
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// معرف نوع الوحدة
    /// Unit type identifier
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
    /// الميزات المخصصة للوحدة (JSON)
    /// Custom features of the unit (JSON)
    /// </summary>
    public string CustomFeatures { get; set; } = string.Empty;

    /// <summary>
    /// طريقة حساب السعر (بالساعة، اليوم، الأسبوع، الشهر)
    /// Pricing calculation method (Hourly, Daily, Weekly, Monthly)
    /// </summary>
    public PricingMethod PricingMethod { get; set; }

    /// <summary>
    /// قيم الحقول الديناميكية للوحدة
    /// Dynamic field values for the unit
    /// </summary>
    public List<FieldValueDto> FieldValues { get; set; } = new List<FieldValueDto>();
} 