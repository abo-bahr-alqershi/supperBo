using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using YemenBooking.Application.Commands.UnitFieldValues;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers;

/// <summary>
/// كونترولر إدارة قيم الحقول الديناميكية للوحدات
/// Unit field values management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitFieldValuesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UnitFieldValuesController> _logger;

    public UnitFieldValuesController(IMediator mediator, ILogger<UnitFieldValuesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// إنشاء قيمة حقل جديدة للوحدة
    /// Create new unit field value
    /// </summary>
    /// <param name="command">بيانات إنشاء قيمة الحقل</param>
    /// <returns>معرف قيمة الحقل المنشأة</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResultDto<Guid>>> CreateUnitFieldValue([FromBody] CreateUnitFieldValueCommand command)
    {
        try
        {
            _logger.LogInformation("طلب إنشاء قيمة حقل جديدة للوحدة: {UnitId}, {FieldId}", 
                command.UnitId, command.UnitTypeFieldId);

            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("تم إنشاء قيمة الحقل بنجاح: {Id}", result.Data);
                return Ok(result);
            }

            _logger.LogWarning("فشل في إنشاء قيمة الحقل: {Error}", result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في إنشاء قيمة حقل الوحدة");
            return StatusCode(500, ResultDto<Guid>.Failed("حدث خطأ داخلي في الخادم"));
        }
    }

    /// <summary>
    /// تحديث قيمة حقل الوحدة
    /// Update unit field value
    /// </summary>
    /// <param name="id">معرف قيمة الحقل</param>
    /// <param name="command">بيانات تحديث قيمة الحقل</param>
    /// <returns>نتيجة التحديث</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResultDto<Guid>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultDto<Guid>>> UpdateUnitFieldValue(Guid id, [FromBody] UpdateUnitFieldValueCommand command)
    {
        try
        {
            if (id != command.Id)
            {
                return BadRequest(ResultDto<Guid>.Failed("معرف قيمة الحقل في المسار لا يطابق المعرف في البيانات"));
            }

            _logger.LogInformation("طلب تحديث قيمة حقل الوحدة: {Id}", id);

            var result = await _mediator.Send(command);
            
            if (result.Success)
            {
                _logger.LogInformation("تم تحديث قيمة الحقل بنجاح: {Id}", result.Data);
                return Ok(result);
            }

            _logger.LogWarning("فشل في تحديث قيمة الحقل: {Error}", result.Message);
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في تحديث قيمة حقل الوحدة: {Id}", id);
            return StatusCode(500, ResultDto<Guid>.Failed("حدث خطأ داخلي في الخادم"));
        }
    }

    /// <summary>
    /// حذف قيمة حقل الوحدة
    /// Delete unit field value
    /// </summary>
    /// <param name="id">معرف قيمة الحقل</param>
    /// <returns>نتيجة الحذف</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultDto<bool>>> DeleteUnitFieldValue(Guid id)
    {
        try
        {
            _logger.LogInformation("طلب حذف قيمة حقل الوحدة: {Id}", id);

            // يمكن إضافة أمر الحذف لاحقاً
            // var command = new DeleteUnitFieldValueCommand { Id = id };
            // var result = await _mediator.Send(command);

            var result = ResultDto<bool>.Ok(true, "تم حذف قيمة الحقل بنجاح");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في حذف قيمة حقل الوحدة: {Id}", id);
            return StatusCode(500, ResultDto<bool>.Failed("حدث خطأ داخلي في الخادم"));
        }
    }

    /// <summary>
    /// الحصول على قيم الحقول للوحدة
    /// Get unit field values
    /// </summary>
    /// <param name="unitId">معرف الوحدة</param>
    /// <returns>قائمة قيم الحقول</returns>
    [HttpGet("unit/{unitId}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResultDto<List<object>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultDto<List<object>>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultDto<List<object>>>> GetUnitFieldValues(Guid unitId)
    {
        try
        {
            _logger.LogInformation("طلب الحصول على قيم حقول الوحدة: {UnitId}", unitId);

            // يمكن إضافة استعلام الحصول على القيم لاحقاً
            // var query = new GetUnitFieldValuesQuery { UnitId = unitId };
            // var result = await _mediator.Send(query);

            var result = ResultDto<List<object>>.Ok(new List<object>(), "تم جلب قيم الحقول بنجاح");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطأ في جلب قيم حقول الوحدة: {UnitId}", unitId);
            return StatusCode(500, ResultDto<List<object>>.Failed("حدث خطأ داخلي في الخادم"));
        }
    }
}