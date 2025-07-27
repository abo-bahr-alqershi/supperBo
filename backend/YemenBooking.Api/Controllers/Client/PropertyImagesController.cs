using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YemenBooking.Application.Queries.PropertyImages;

namespace YemenBooking.Api.Controllers.Client
{
    /// <summary>
    /// متحكم بعرض صور الكيانات للعملاء
    /// Controller for clients to view property images
    /// </summary>
    public class PropertyImagesController : BaseClientController
    {
        public PropertyImagesController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// جلب صور الكيانات
        /// Get property images with filters and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPropertyImages([FromQuery] GetPropertyImagesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
} 