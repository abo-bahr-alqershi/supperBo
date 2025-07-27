using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Application.DTOs;
using AutoMapper;
using YemenBooking.Core.Entities;
using AvailabilitySearchDto = YemenBooking.Application.DTOs.AvailabilitySearchRequest;

namespace YemenBooking.Api.Controllers
{
    /// <summary>
    /// متحكم لإدارة الإتاحة
    /// Controller for managing unit availability
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AvailabilityController : ControllerBase
    {
        private readonly IUnitAvailabilityRepository _availabilityRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly IMapper _mapper;

        public AvailabilityController(
            IUnitAvailabilityRepository availabilityRepository,
            IAvailabilityService availabilityService,
            IMapper mapper)
        {
            _availabilityRepository = availabilityRepository;
            _availabilityService = availabilityService;
            _mapper = mapper;
        }

        /// <summary>
        /// جلب إتاحة الوحدة لفترة معينة
        /// Get unit availability within date range
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUnitAvailability(
            [FromQuery] Guid unitId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var availabilities = await _availabilityRepository.FindAsync(x =>
                x.UnitId == unitId
                && (!startDate.HasValue || x.StartDate >= startDate.Value)
                && (!endDate.HasValue || x.EndDate <= endDate.Value));
            var dtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(availabilities);
            return Ok(new { data = dtos });
        }

        /// <summary>
        /// إنشاء إتاحة جديدة
        /// Create a new unit availability
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAvailability(
            [FromBody] CreateAvailabilityRequestDto request)
        {
            if (request.Status.Equals("unavailable", StringComparison.OrdinalIgnoreCase)
                && request.OverrideConflicts != true
                && !(await _availabilityService.CheckAvailabilityAsync(
                    request.UnitId,
                    request.StartDate,
                    request.EndDate)))
            {
                return BadRequest(new { message = "لا يمكن إنشاء الإتاحة؛ توجد حجوزات متداخلة في هذه الفترة" });
            }
            var entity = new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = request.UnitId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                Reason = request.Reason,
                Notes = request.Notes
            };
            await _availabilityRepository.AddAsync(entity);
            var dto = _mapper.Map<UnitAvailabilityDetailDto>(entity);
            return Ok(new { data = dto });
        }

        /// <summary>
        /// البحث في الإتاحات
        /// Search for unit availabilities
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchAvailability(
            [FromBody] AvailabilitySearchDto request)
        {
            var availabilities = await _availabilityRepository.FindAsync(x =>
                (request.UnitIds == null || request.UnitIds.Contains(x.UnitId))
                && (!request.StartDate.HasValue || x.StartDate >= request.StartDate.Value)
                && (!request.EndDate.HasValue || x.EndDate <= request.EndDate.Value)
                && (request.Statuses == null || request.Statuses.Contains(x.Status)));
            var dtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(availabilities);
            return Ok(new
            {
                availabilities = dtos,
                conflicts = new object[0],
                total_count = dtos.Count(),
                has_more = false
            });
        }

        /// <summary>
        /// تحديث إتاحة موجودة
        /// Update an existing unit availability
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(
            [FromRoute(Name = "id")] Guid id,
            [FromBody] UpdateAvailabilityRequestDto request)
        {
            var entity = await _availabilityRepository.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            if (request.Status.Equals("unavailable", StringComparison.OrdinalIgnoreCase)
                && request.OverrideConflicts != true
                && !(await _availabilityService.CheckAvailabilityAsync(
                    entity.UnitId,
                    request.StartDate,
                    request.EndDate)))
            {
                return BadRequest(new { message = "لا يمكن تحديث الإتاحة؛ توجد حجوزات متداخلة في هذه الفترة" });
            }

            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.Status = request.Status;
            entity.Reason = request.Reason;
            entity.Notes = request.Notes;

            await _availabilityRepository.UpdateAsync(entity);

            var resultDto = _mapper.Map<UnitAvailabilityDetailDto>(entity);
            return Ok(new { data = resultDto });
        }

        /// <summary>
        /// حذف إتاحة
        /// Delete a unit availability
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(Guid id)
        {
            var exists = await _availabilityRepository.ExistsAsync(id);
            if (!exists)
                return NotFound();
            await _availabilityRepository.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// إنشاء إتاحة مجمعة
        /// Bulk create unit availabilities
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreateAvailability(
            [FromBody] BulkAvailabilityRequestDto request)
        {
            var conflicts = new List<object>();
            foreach (var dto in request.Requests)
            {
                if (dto.Status.Equals("unavailable", StringComparison.OrdinalIgnoreCase)
                    && dto.OverrideConflicts != true
                    && !(await _availabilityService.CheckAvailabilityAsync(
                        dto.UnitId,
                        dto.StartDate,
                        dto.EndDate)))
                {
                    conflicts.Add(new { dto.UnitId, dto.StartDate, dto.EndDate });
                }
            }
            if (conflicts.Any())
            {
                return BadRequest(new { message = "لا يمكن إنشاء الإتاحات؛ توجد حجوزات متداخلة في بعض الفترات", conflicts });
            }
            var entities = request.Requests.Select(dto => new UnitAvailability
            {
                Id = Guid.NewGuid(),
                UnitId = dto.UnitId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                Reason = dto.Reason,
                Notes = dto.Notes
            }).ToList();
            await _availabilityRepository.AddRangeAsync(entities);
            var resultDtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(entities);
            return Ok(new { data = resultDtos });
        }

        /// <summary>
        /// تحديث سريع لحالة الإتاحة
        /// Quick update availability status for a unit
        /// </summary>
        [HttpPatch("quick-update/{unitId}")]
        public async Task<IActionResult> QuickUpdateStatus(
            [FromRoute] Guid unitId,
            [FromBody] QuickUpdateAvailabilityRequestDto request)
        {
            if (!request.Status.Equals("available", StringComparison.OrdinalIgnoreCase)
                && !(await _availabilityService.CheckAvailabilityAsync(
                    unitId,
                    request.StartDate,
                    request.EndDate)))
            {
                return BadRequest(new { message = "لا يمكن حظر الإتاحة؛ توجد حجوزات متداخلة في هذه الفترة" });
            }
            await _availabilityRepository.UpdateAvailabilityAsync(unitId, request.StartDate, request.EndDate, request.Status == "available");
            var overrides = await _availabilityRepository.FindAsync(x => x.UnitId == unitId && x.StartDate >= request.StartDate && x.EndDate <= request.EndDate);
            var resultDtos = _mapper.Map<IEnumerable<UnitAvailabilityDetailDto>>(overrides);
            return Ok(new { data = resultDtos });
        }
    }
} 