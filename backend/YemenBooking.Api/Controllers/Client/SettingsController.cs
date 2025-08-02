using MediatR;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.Settings;
using YemenBooking.Application.Queries.MobileApp.Settings;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر إعدادات المستخدم للعملاء
    /// Client User Settings Controller
    /// </summary>
    public class SettingsController : BaseClientController
    {
        public SettingsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// تحديث إعدادات المستخدم
        /// Update user settings
        /// </summary>
        /// <param name="command">الإعدادات الجديدة</param>
        /// <returns>نتيجة التحديث</returns>
        [HttpPut]
        public async Task<ActionResult<ResultDto<UpdateUserSettingsResponse>>> UpdateUserSettings([FromBody] UpdateUserSettingsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على إعدادات المستخدم الحالية
        /// Get current user settings
        /// </summary>
        /// <param name="query">معايير الاستعلام</param>
        /// <returns>إعدادات المستخدم</returns>
        [HttpGet]
        public async Task<ActionResult<ResultDto<YemenBooking.Application.DTOs.Users.UserSettingsDto>>> GetUserSettings([FromQuery] GetUserSettingsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
