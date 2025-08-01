using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using YemenBooking.Application.Queries;
using YemenBooking.Application.Commands;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Api.Controllers
{
    [ApiController]
    [Route("api/home-screens")]
    public class HomeSectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HomeSectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// استرجاع معاينة الشاشة الرئيسية استنادًا إلى قالب محدد
        /// </summary>
        /// <param name="templateId">معرّف القالب</param>
        /// <param name="platform">المنصة (اختياري)</param>
        /// <param name="deviceType">نوع الجهاز (اختياري)</param>
        /// <param name="useMockData">استخدام بيانات وهمية؟</param>
        /// <returns>معاينة الشاشة الرئيسية</returns>
        [HttpGet("preview")]
        public async Task<ActionResult<ResultDto<HomeScreenPreviewDto>>> PreviewHomeScreen(
            [FromQuery] Guid templateId,
            [FromQuery] string platform,
            [FromQuery] string deviceType,
            [FromQuery] bool useMockData = false)
        {
            var query = new PreviewHomeScreenQuery
            {
                TemplateId = templateId,
                Platform = platform,
                DeviceType = deviceType,
                UseMockData = useMockData
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع جميع قوالب الشاشة الرئيسية
        /// </summary>
        /// <param name="platform">المنصة (اختياري)</param>
        /// <param name="targetAudience">الجمهور المستهدف (اختياري)</param>
        /// <param name="isActive">فلترة القوالب النشطة (اختياري)</param>
        /// <param name="includeDeleted">تضمين المحذوفة؟</param>
        /// <returns>قائمة قوالب الشاشة الرئيسية</returns>
        [HttpGet("templates")]
        public async Task<ActionResult<ResultDto<List<HomeScreenTemplateDto>>>> GetTemplates(
            [FromQuery] string platform,
            [FromQuery] string targetAudience,
            [FromQuery] bool? isActive,
            [FromQuery] bool includeDeleted = false)
        {
            var query = new GetHomeScreenTemplatesQuery
            {
                Platform = platform,
                TargetAudience = targetAudience,
                IsActive = isActive,
                IncludeDeleted = includeDeleted
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع قالب شاشة رئيسية حسب المعرف
        /// </summary>
        /// <param name="id">معرّف القالب</param>
        /// <param name="includeHierarchy">تضمين الهيكل الكامل؟</param>
        /// <returns>القالب المطلوب مع الهيكلية</returns>
        [HttpGet("templates/{id}")]
        public async Task<ActionResult<ResultDto<HomeScreenTemplateDto>>> GetTemplateById(
            Guid id,
            [FromQuery] bool includeHierarchy = false)
        {
            var query = new GetHomeScreenTemplateByIdQuery
            {
                TemplateId = id,
                IncludeHierarchy = includeHierarchy
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء قالب شاشة رئيسية جديد
        /// </summary>
        /// <param name="command">بيانات إنشاء القالب</param>
        /// <returns>القالب المنشأ</returns>
        [HttpPost("templates")]
        public async Task<ActionResult<ResultDto<HomeScreenTemplateDto>>> CreateTemplate(
            [FromBody] CreateHomeScreenTemplateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات قالب شاشة رئيسية
        /// </summary>
        /// <param name="id">معرّف القالب</param>
        /// <param name="command">بيانات التحديث</param>
        /// <returns>القالب المحدث</returns>
        [HttpPut("templates/{id}")]
        public async Task<ActionResult<ResultDto<HomeScreenTemplateDto>>> UpdateTemplate(
            Guid id,
            [FromBody] UpdateHomeScreenTemplateCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف قالب شاشة رئيسية
        /// </summary>
        /// <param name="id">معرّف القالب</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpDelete("templates/{id}")]
        public async Task<ActionResult<ResultDto<bool>>> DeleteTemplate(Guid id)
        {
            var command = new DeleteHomeScreenTemplateCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// نسخ قالب شاشة رئيسية
        /// </summary>
        /// <param name="templateId">معرّف القالب الأصلي</param>
        /// <param name="newName">الاسم الجديد للقالب المنسوخ (اختياري)</param>
        /// <param name="newDescription">الوصف الجديد (اختياري)</param>
        /// <returns>القالب المنسوخ</returns>
        [HttpPost("templates/{templateId}/duplicate")]
        public async Task<ActionResult<ResultDto<HomeScreenTemplateDto>>> DuplicateTemplate(
            Guid templateId,
            [FromQuery] string newName = null,
            [FromQuery] string newDescription = null)
        {
            var command = new DuplicateTemplateCommand
            {
                TemplateId = templateId,
                NewName = newName,
                NewDescription = newDescription
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// نشر قالب شاشة رئيسية
        /// </summary>
        /// <param name="templateId">معرّف القالب</param>
        /// <param name="deactivateOthers">إلغاء تفعيل القوالب الأخرى؟</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpPost("templates/{templateId}/publish")]
        public async Task<ActionResult<ResultDto<bool>>> PublishTemplate(
            Guid templateId,
            [FromQuery] bool deactivateOthers = false)
        {
            var command = new PublishTemplateCommand
            {
                TemplateId = templateId,
                DeactivateOthers = deactivateOthers
            };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع مصادر البيانات المتاحة لنوع مكون معين
        /// </summary>
        /// <param name="componentType">نوع المكون</param>
        /// <returns>قائمة مصادر البيانات</returns>
        [HttpGet("data-sources")]
        public async Task<ActionResult<ResultDto<List<DataSourceDto>>>> GetDataSources(
            [FromQuery] string componentType)
        {
            var query = new GetDataSourcesQuery { ComponentType = componentType };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع أنواع المكونات المتاحة
        /// </summary>
        /// <param name="platform">المنصة (اختياري)</param>
        /// <returns>قائمة أنواع المكونات</returns>
        [HttpGet("component-types")]
        public async Task<ActionResult<ResultDto<List<ComponentTypeDto>>>> GetComponentTypes(
            [FromQuery] string platform)
        {
            var query = new GetComponentTypesQuery { Platform = platform };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// استرجاع القالب النشط للشاشة الرئيسية
        /// </summary>
        /// <param name="platform">المنصة (اختياري)</param>
        /// <param name="targetAudience">الجمهور المستهدف (اختياري)</param>
        /// <param name="userId">معرّف المستخدم (اختياري)</param>
        /// <returns>القالب النشط</returns>
        [HttpGet("active")]
        public async Task<ActionResult<ResultDto<HomeScreenTemplateDto>>> GetActiveTemplate(
            [FromQuery] string platform,
            [FromQuery] string targetAudience,
            [FromQuery] Guid? userId = null)
        {
            var query = new GetActiveHomeScreenQuery
            {
                Platform = platform,
                TargetAudience = targetAudience,
                UserId = userId
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء قسم جديد في الشاشة الرئيسية
        /// </summary>
        /// <param name="command">بيانات القسم</param>
        /// <returns>القسم المنشأ</returns>
        [HttpPost("sections")]
        public async Task<ActionResult<ResultDto<HomeScreenSectionDto>>> CreateSection(
            [FromBody] CreateHomeScreenSectionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات قسم في الشاشة الرئيسية
        /// </summary>
        /// <param name="id">معرّف القسم</param>
        /// <param name="command">بيانات التحديث</param>
        /// <returns>القسم المحدث</returns>
        [HttpPut("sections/{id}")]
        public async Task<ActionResult<ResultDto<HomeScreenSectionDto>>> UpdateSection(
            Guid id,
            [FromBody] UpdateHomeScreenSectionCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف قسم من الشاشة الرئيسية
        /// </summary>
        /// <param name="id">معرّف القسم</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpDelete("sections/{id}")]
        public async Task<ActionResult<ResultDto<bool>>> DeleteSection(Guid id)
        {
            var command = new DeleteHomeScreenSectionCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إعادة ترتيب الأقسام في قالب معين
        /// </summary>
        /// <param name="request">قائمة ترتيب الأقسام الجديدة</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpPost("sections/reorder")]
        public async Task<ActionResult<ResultDto<bool>>> ReorderSections(
            [FromBody] ReorderSectionsCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// إنشاء مكون جديد في قسم
        /// </summary>
        /// <param name="command">بيانات المكون</param>
        /// <returns>معرّف المكون المنشأ</returns>
        [HttpPost("components")]
        public async Task<ActionResult<ResultDto<Guid>>> CreateComponent(
            [FromBody] CreateHomeScreenComponentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات مكون في الشاشة الرئيسية
        /// </summary>
        /// <param name="id">معرّف المكون</param>
        /// <param name="command">بيانات التحديث</param>
        /// <returns>المكون المحدث</returns>
        [HttpPut("components/{id}")]
        public async Task<ActionResult<ResultDto<HomeScreenComponentDto>>> UpdateComponent(
            Guid id,
            [FromBody] UpdateHomeScreenComponentCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// حذف مكون من الشاشة الرئيسية
        /// </summary>
        /// <param name="id">معرّف المكون</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpDelete("components/{id}")]
        public async Task<ActionResult<ResultDto<bool>>> DeleteComponent(Guid id)
        {
            var command = new DeleteHomeScreenComponentCommand { Id = id };
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// إعادة ترتيب المكونات في قسم
        /// </summary>
        /// <param name="request">قائمة ترتيب المكونات الجديدة</param>
        /// <returns>نجاح أو فشل العملية</returns>
        [HttpPost("components/reorder")]
        public async Task<ActionResult<ResultDto<bool>>> ReorderComponents(
            [FromBody] ReorderComponentsCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}