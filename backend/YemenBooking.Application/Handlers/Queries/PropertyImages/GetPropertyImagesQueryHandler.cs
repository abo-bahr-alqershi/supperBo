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
using Microsoft.EntityFrameworkCore;

namespace YemenBooking.Application.Handlers.Queries.PropertyImages
{
    /// <summary>
    /// معالج استعلام الحصول على صور الكيان
    /// Query handler for GetPropertyImagesQuery
    /// </summary>
    public class GetPropertyImagesQueryHandler : IRequestHandler<GetPropertyImagesQuery, ResultDto<PaginatedResult<PropertyImageDto>>>
    {
        private readonly IPropertyImageRepository _repo;
        private readonly ILogger<GetPropertyImagesQueryHandler> _logger;

        public GetPropertyImagesQueryHandler(IPropertyImageRepository repo, ILogger<GetPropertyImagesQueryHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<ResultDto<PaginatedResult<PropertyImageDto>>> Handle(GetPropertyImagesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("جاري معالجة استعلام صور الكيان: PropertyId={PropertyId}, UnitId={UnitId}, PageNumber={PageNumber}, PageSize={PageSize}", request.PropertyId, request.UnitId, request.PageNumber, request.PageSize);

            var queryable = _repo.GetQueryable();

            if (request.PropertyId.HasValue)
                queryable = queryable.Where(img => img.PropertyId == request.PropertyId.Value);

            if (request.UnitId.HasValue)
                queryable = queryable.Where(img => img.UnitId == request.UnitId.Value);

            var totalCount = await queryable.CountAsync(cancellationToken);

            var images = await queryable
                .OrderBy(img => img.DisplayOrder)
                .ThenBy(img => img.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

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
                Status = img.Status,
                AssociationType = img.PropertyId.HasValue ? "Property" : img.UnitId.HasValue ? "Unit" : string.Empty
            }).ToList();

            var pagedResult = PaginatedResult<PropertyImageDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);

            return ResultDto<PaginatedResult<PropertyImageDto>>.Ok(pagedResult, "تم جلب صور الكيان بنجاح");
        }
    }
} 