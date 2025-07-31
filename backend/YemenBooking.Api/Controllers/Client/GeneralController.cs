using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Commands.MobileApp.General;
using YemenBooking.Application.Queries.MobileApp.General;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// كونترولر الوظائف العامة للعملاء
    /// Client General Functions Controller
    /// </summary>
    public class GeneralController : BaseClientController
    {
        public GeneralController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// إرسال ملاحظات أو اقتراحات
        /// Submit feedback or suggestions
        /// </summary>
        /// <param name="command">بيانات الملاحظات</param>
        /// <returns>نتيجة الإرسال</returns>
        [HttpPost("feedback")]
        public async Task<ActionResult<ResultDto<SubmitFeedbackResponse>>> SubmitFeedback([FromBody] SubmitFeedbackCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على إصدار التطبيق الحالي
        /// Get current app version
        /// </summary>
        /// <returns>معلومات الإصدار</returns>
        [HttpGet("app-version")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<AppVersionResponse>>> GetAppVersion()
        {
            var query = new GetAppVersionQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على سعر صرف العملة
        /// Get currency exchange rate
        /// </summary>
        /// <param name="query">معايير العملة</param>
        /// <returns>سعر الصرف</returns>
        [HttpGet("currency-exchange")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<CurrencyExchangeRateResponse>>> GetCurrencyExchangeRate([FromQuery] GetCurrencyExchangeRateQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الأسئلة الشائعة
        /// Get frequently asked questions
        /// </summary>
        /// <returns>قائمة الأسئلة الشائعة</returns>
        [HttpGet("faqs")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<FAQsResponse>>> GetFAQs()
        {
            var query = new GetFAQsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على سياسة الخصوصية
        /// Get privacy policy
        /// </summary>
        /// <returns>سياسة الخصوصية</returns>
        [HttpGet("privacy-policy")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<PrivacyPolicyResponse>>> GetPrivacyPolicy()
        {
            var query = new GetPrivacyPolicyQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الشروط والأحكام
        /// Get terms and conditions
        /// </summary>
        /// <returns>الشروط والأحكام</returns>
        [HttpGet("terms-and-conditions")]
        [AllowAnonymous]
        public async Task<ActionResult<ResultDto<TermsAndConditionsResponse>>> GetTermsAndConditions()
        {
            var query = new GetTermsAndConditionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}