using MediatR;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Commands.Units;

/// <summary>
/// أمر لتحديث حالة توفر الوحدة
/// Command to update unit availability status
/// </summary>
public class UpdateUnitAvailabilityCommand : IRequest<ResultDto<bool>>
{
    /// <summary>
    /// معرف الوحدة
    /// Unit ID
    /// </summary>
    public Guid UnitId { get; set; }

    /// <summary>
    /// حالة التوفر
    /// Availability status
    /// </summary>
    public bool IsAvailable { get; set; }
} 