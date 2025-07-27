using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using YemenBooking.Application.Commands.Images;
using YemenBooking.Application.DTOs;
using YemenBooking.Core.Interfaces;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Enums;
using System.Collections.Generic;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using System.Text.Json;

namespace YemenBooking.Application.Handlers.Commands.Images
{
    /// <summary>
    /// معالج أمر رفع صورة مع بيانات إضافية
    /// </summary>
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, ResultDto<ImageDto>>
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;
        private readonly ILogger<UploadImageCommandHandler> _logger;
        private readonly IPropertyImageRepository _imageRepository;
        private readonly IUnitRepository _unitRepository;

        public UploadImageCommandHandler(
            IFileStorageService fileStorageService,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUserService,
            IAuditService auditService,
            ILogger<UploadImageCommandHandler> logger,
            IPropertyImageRepository imageRepository,
            IUnitRepository unitRepository)
        {
            _fileStorageService = fileStorageService;
            _imageProcessingService = imageProcessingService;
            _currentUserService = currentUserService;
            _auditService = auditService;
            _logger = logger;
            _imageRepository = imageRepository;
            _unitRepository = unitRepository;
        }

        /// <inheritdoc />
        public async Task<ResultDto<ImageDto>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("بدء رفع الصورة: Name={Name}, Type={Type}", request.Name, request.ImageType);

            // التحقق من المصادقة
            if (_currentUserService.UserId == Guid.Empty)
                return ResultDto<ImageDto>.Failed("يجب تسجيل الدخول لرفع الصور");

            // التحقق من صحة المدخلات
            if (request.File == null || request.File.FileContent == null || request.File.FileContent.Length == 0)
                return ResultDto<ImageDto>.Failed("ملف الصورة مطلوب");
            if (string.IsNullOrWhiteSpace(request.Name))
                return ResultDto<ImageDto>.Failed("اسم الملف مطلوب");
            if (string.IsNullOrWhiteSpace(request.Extension))
                return ResultDto<ImageDto>.Failed("امتداد الملف مطلوب");

            try
            {
                // تحويل المحتوى إلى تيار
                var stream = new MemoryStream(request.File.FileContent);

                // Determine dynamic folder path based on ImageType, PropertyId, and UnitId
                Guid effectivePropertyId = request.PropertyId ?? Guid.Empty;
                if (effectivePropertyId == Guid.Empty && request.UnitId.HasValue)
                {
                    var unitEntity = await _unitRepository.GetUnitByIdAsync(request.UnitId.Value, cancellationToken);
                    effectivePropertyId = unitEntity?.PropertyId ?? Guid.Empty;
                }
                string folderPath;
                if (effectivePropertyId == Guid.Empty)
                {
                    folderPath = "temp";
                }
                else
                {
                    folderPath = $"{request.ImageType}/{effectivePropertyId}";
                    if (request.UnitId.HasValue)
                        folderPath = $"{folderPath}/{request.UnitId.Value}";
                }

                // التحقق من صلاحية الصورة
                stream.Seek(0, SeekOrigin.Begin);
                var validationOptions = new ImageValidationOptions
                {
                    MaxFileSizeBytes = 5 * 1024 * 1024 // 5 ميغابايت كحد أقصى
                };
                var validationResult = await _imageProcessingService.ValidateImageAsync(stream, validationOptions, cancellationToken);
                if (!validationResult.IsValid)
                    return ResultDto<ImageDto>.Failed(validationResult.ValidationErrors, "فشل التحقق من صحة الصورة");

                // تحسين الصورة إذا طُلب
                if (request.OptimizeImage)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var compressResult = await _imageProcessingService.CompressImageAsync(stream, request.Quality ?? 85, null, cancellationToken);
                    if (compressResult.IsSuccess && compressResult.ProcessedImageBytes != null)
                    {
                        stream.Dispose();
                        stream = new MemoryStream(compressResult.ProcessedImageBytes);
                    }
                    else if (!compressResult.IsSuccess)
                    {
                        _logger.LogWarning("فشل تحسين الصورة: {Error}", compressResult.ErrorMessage);
                    }
                }

                // إنشاء صورة مصغرة إذا طُلب
                if (request.GenerateThumbnail)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var thumbResult32 = await _imageProcessingService.GenerateThumbnailAsync(stream, maxHeight: 32, maxWidth: 32, cancellationToken: cancellationToken);
                    if (thumbResult32.IsSuccess && thumbResult32.ProcessedImageBytes != null)
                    {
                        var thumbName = $"{request.Name}_thumb{request.Extension}";
                        await _fileStorageService.UploadFileAsync(
                            thumbResult32.ProcessedImageBytes,
                            thumbName,
                            request.File.ContentType,
                            folderPath,
                            cancellationToken);
                    }
                    else if (!thumbResult32.IsSuccess)
                    {
                        _logger.LogWarning("فشل إنشاء الصورة المصغرة: {Error}", thumbResult32.ErrorMessage);
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    var thumbResult48 = await _imageProcessingService.GenerateThumbnailAsync(stream, maxHeight: 48, maxWidth: 48, cancellationToken: cancellationToken);
                    if (thumbResult48.IsSuccess && thumbResult48.ProcessedImageBytes != null)
                    {
                        var thumbName = $"{request.Name}_thumb48{request.Extension}";
                        await _fileStorageService.UploadFileAsync(
                            thumbResult48.ProcessedImageBytes,
                            thumbName,
                            request.File.ContentType,
                            folderPath,
                            cancellationToken);
                    }
                    else if (!thumbResult48.IsSuccess)
                    {
                        _logger.LogWarning("فشل إنشاء الصورة المصغرة: {Error}", thumbResult48.ErrorMessage);
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    var thumbResult64 = await _imageProcessingService.GenerateThumbnailAsync(stream, maxHeight: 64, maxWidth: 64, cancellationToken: cancellationToken);
                    if (thumbResult64.IsSuccess && thumbResult64.ProcessedImageBytes != null)
                    {
                        var thumbName = $"{request.Name}_thumb64{request.Extension}";
                        await _fileStorageService.UploadFileAsync(
                            thumbResult64.ProcessedImageBytes,
                            thumbName,
                            request.File.ContentType,
                            folderPath,
                            cancellationToken);
                    }
                    else if (!thumbResult64.IsSuccess)
                    {
                        _logger.LogWarning("فشل إنشاء الصورة المصغرة: {Error}", thumbResult64.ErrorMessage);
                    }
                }

                // رفع الملف الرئيسي
                stream.Seek(0, SeekOrigin.Begin);
                var fileName = request.Name + request.Extension;
                var uploadResult = await _fileStorageService.UploadFileAsync(
                    stream,
                    fileName,
                    request.File.ContentType,
                    folderPath,
                    cancellationToken);

                if (!uploadResult.IsSuccess || uploadResult.FileUrl == null)
                    return ResultDto<ImageDto>.Failed("حدث خطأ أثناء رفع الصورة");

                // تسجيل عملية الرفع في السجل
                await _auditService.LogBusinessOperationAsync(
                    "UploadImage",
                    $"تم رفع الصورة {fileName} من قبل المستخدم {_currentUserService.UserId}",
                    null,
                    "Image",
                    _currentUserService.UserId,
                    new System.Collections.Generic.Dictionary<string, object>
                    {
                        { "Path", uploadResult.FilePath }
                    },
                    cancellationToken);

                _logger.LogInformation("اكتمل رفع الصورة بنجاح: Url={Url}", uploadResult.FileUrl);
                // بناء DTO للصورة للرد
                var imageDto = new ImageDto
                {
                    Id = Guid.NewGuid(),
                    Url = uploadResult.FileUrl,
                    Filename = fileName,
                    Size = uploadResult.FileSizeBytes,
                    MimeType = request.File.ContentType,
                    Width = 0,
                    Height = 0,
                    Alt = request.Alt,
                    UploadedAt = DateTime.UtcNow,
                    UploadedBy = _currentUserService.UserId,
                    Order = request.Order ?? 0,
                    IsPrimary = request.IsPrimary ?? false,
                    PropertyId = request.PropertyId,
                    UnitId = request.UnitId,
                    Category = request.Category,
                    Tags = request.Tags ?? new List<string>(),
                    ProcessingStatus = "ready",
                    Thumbnails = new ImageThumbnailsDto()
                };
                // Determine PropertyId association: maintain null if none
                Guid? propertyAssociation = request.PropertyId;
                if (!propertyAssociation.HasValue && request.UnitId.HasValue)
                {
                    var unit = await _unitRepository.GetUnitByIdAsync(request.UnitId.Value, cancellationToken);
                    propertyAssociation = unit?.PropertyId;
                }

                // Persist image entity to database
                var imageEntity = new PropertyImage
                {
                    Id = imageDto.Id,
                    PropertyId = propertyAssociation,
                    UnitId = request.UnitId,
                    Name = fileName,
                    Url = uploadResult.FileUrl,
                    SizeBytes = uploadResult.FileSizeBytes,
                    Type = request.File.ContentType,
                    Category = request.Category,
                    Caption = request.Alt ?? string.Empty,
                    AltText = request.Alt ?? string.Empty,
                    Tags = JsonSerializer.Serialize(request.Tags ?? new List<string>()),
                    Sizes = uploadResult.FileUrl,
                    IsMainImage = request.IsPrimary ?? false,
                    DisplayOrder = request.Order ?? 0,
                    Status = ImageStatus.Approved,
                    UploadedAt = imageDto.UploadedAt,
                    CreatedBy = _currentUserService.UserId,
                    UpdatedAt = imageDto.UploadedAt
                };
                await _imageRepository.CreatePropertyImageAsync(imageEntity, cancellationToken);
                
                return ResultDto<ImageDto>.Succeeded(imageDto, "تم رفع الصورة بنجاح");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في رفع الصورة");
                return ResultDto<ImageDto>.Failed("حدث خطأ غير متوقع أثناء رفع الصورة");
            }
        }
    }
} 