namespace YemenBooking.Application.Queries.UnitTypeFields;

using System;
using MediatR;
using YemenBooking.Application.DTOs;

/// <summary>
/// جلب حقل نوع الوحدة حسب المعرف
/// Query for getting a property type field by its identifier
/// </summary>
public class GetUnitTypeFieldByIdQuery : IRequest<ResultDto<UnitTypeFieldDto>>
{
    /// <summary>
    /// Identifier of the property type field
    /// </summary>
    public Guid FieldId { get; set; }
} 