using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace YemenBooking.Api.Controllers
{
    /// <summary>
    /// متحكم التحقق من التعارضات
    /// Controller for conflict validation and resolution
    /// </summary>
    [ApiController]
    [Route("api/validation")]
    public class ValidationController : ControllerBase
    {
        /// <summary>
        /// التحقق من التعارضات
        /// Check for booking conflicts
        /// </summary>
        [HttpPost("check-conflicts")]
        public async Task<IActionResult> CheckConflicts([FromBody] ConflictCheckRequestDto request)
        {
            // Stub: no conflicts
            var response = new ConflictCheckResponseDto
            {
                HasConflicts = false,
                Conflicts = new List<BookingConflictDto>(),
                Recommendations = new List<RecommendationDto>()
            };
            return Ok(response);
        }

        /// <summary>
        /// الحصول على التعارضات لوحدة
        /// Get unit conflicts
        /// </summary>
        [HttpGet("conflicts")]
        public async Task<IActionResult> GetUnitConflicts(
            [FromQuery(Name = "unit_id")] Guid unitId,
            [FromQuery(Name = "start_date")] DateTime? startDate,
            [FromQuery(Name = "end_date")] DateTime? endDate)
        {
            return Ok(new { data = new List<BookingConflictDto>() });
        }

        /// <summary>
        /// حل التعارض
        /// Resolve a conflict
        /// </summary>
        [HttpPost("resolve-conflict/{conflictId}")]
        public async Task<IActionResult> ResolveConflict(
            [FromRoute] Guid conflictId,
            [FromBody] ResolveConflictRequestDto request)
        {
            return Ok(new { success = true, message = "تم حل التعارض بنجاح" });
        }

        /// <summary>
        /// الحصول على إعدادات التحقق
        /// Get validation settings
        /// </summary>
        [HttpGet("settings")]
        public IActionResult GetValidationSettings()
        {
            var settings = new ValidationSettingsDto();
            return Ok(new { data = settings });
        }

        /// <summary>
        /// تحديث إعدادات التحقق
        /// Update validation settings
        /// </summary>
        [HttpPut("settings")]
        public IActionResult UpdateValidationSettings([FromBody] ValidationSettingsDto settings)
        {
            return Ok(new { data = settings });
        }

        // DTOs
        public class ConflictCheckRequestDto
        {
            public Guid UnitId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public string CheckType { get; set; }
        }

        /// <summary>
        /// استجابة التحقق من التعارضات
        /// Conflict check response DTO
        /// </summary>
        public class ConflictCheckResponseDto
        {
            public bool HasConflicts { get; set; }
            public IEnumerable<BookingConflictDto> Conflicts { get; set; }
            public IEnumerable<RecommendationDto> Recommendations { get; set; }
        }

        public class BookingConflictDto
        {
            public Guid ConflictId { get; set; }
            public Guid UnitId { get; set; }
            public Guid BookingId { get; set; }
            public string GuestName { get; set; }
            public string GuestEmail { get; set; }
            public string GuestPhone { get; set; }
            public DateTime BookingStart { get; set; }
            public DateTime BookingEnd { get; set; }
            public string BookingStatus { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentStatus { get; set; }
            public string ConflictType { get; set; }
            public string ImpactLevel { get; set; }
            public IEnumerable<RecommendationDto> Recommendations { get; set; }
            public bool CanModify { get; set; }
            public DateTime? ModificationDeadline { get; set; }
        }

        public class RecommendationDto
        {
            public string Action { get; set; }
            public string Description { get; set; }
            public bool Feasible { get; set; }
            public decimal? EstimatedCost { get; set; }
        }

        public class ResolveConflictRequestDto
        {
            public string Action { get; set; }
            public string? Notes { get; set; }
        }

        public class ValidationSettingsDto
        {
            public int MinAdvanceNoticeHours { get; set; }
            public int MaxFutureDateMonths { get; set; }
            public bool AllowPastDateModification { get; set; }
            public bool RequireConflictResolution { get; set; }
            public bool AutoUpdateDependentPrices { get; set; }
            public int PriceChangeNotificationThreshold { get; set; }
            public Dictionary<string, decimal> MinPricePerUnitType { get; set; }
            public Dictionary<string, decimal> MaxPricePerUnitType { get; set; }
        }
    }
} 