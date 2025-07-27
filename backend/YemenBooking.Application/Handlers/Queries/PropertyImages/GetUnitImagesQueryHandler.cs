using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Queries.PropertyImages;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Application.Exceptions;

namespace YemenBooking.Application.Handlers.Queries.PropertyImages
{
    /// <summary>
    /// معالج استعلام الحصول على صور الوحدة
    /// Query handler for GetUnitImagesQuery
    /// </summary>
    public class GetUnitImagesQueryHandler : IRequestHandler<GetUnitImagesQuery, ResultDto<IEnumerable<PropertyImageDto>>>
    {
        private readonly IPropertyImageRepository _repo;
        private readonly ILogger<GetUnitImagesQueryHandler> _logger;

        public GetUnitImagesQueryHandler(IPropertyImageRepository repo, ILogger<GetUnitImagesQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ResultDto<IEnumerable<PropertyImageDto>>> Handle(GetUnitImagesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام صور الوحدة: {UnitId}", request.UnitId);

            if (request.UnitId == Guid.Empty)
                throw new ValidationException(nameof(request.UnitId), "معرف الوحدة غير صالح");

            var images = await _repo.GetImagesByUnitAsync(request.UnitId, cancellationToken);

            var dtos = images.Select(img => new PropertyImageDto
            {
                Id = img.Id,
                PropertyId = img.PropertyId,
                UnitId = img.UnitId,
                Name = img.Name,
                Url = img.Url,
                SizeBytes = img.SizeBytes,
                Type = img.Type,
                Category = img.Category,
                Caption = img.Caption,
                AltText = img.AltText,
                Tags = img.Tags,
                Sizes = img.Sizes,
                IsMain = img.IsMain,
                DisplayOrder = img.DisplayOrder,
                UploadedAt = img.UploadedAt,
                Status = img.Status
            }).ToList();

            return ResultDto<IEnumerable<PropertyImageDto>>.Ok(dtos, "تم جلب صور الوحدة بنجاح");
        }
    }
} 