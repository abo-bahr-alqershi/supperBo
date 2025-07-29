using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Interfaces.Services;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// خدمة رفع الملفات
    /// Stub implementation of IFileUploadService
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        public Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder = "uploads")
        {
            throw new NotImplementedException("FileUploadService.UploadFileAsync is not implemented yet.");
        }

        public Task<List<string>> UploadFilesAsync(List<(Stream stream, string fileName, string contentType)> files, string folder = "uploads")
        {
            throw new NotImplementedException("FileUploadService.UploadFilesAsync is not implemented yet.");
        }

        public Task<string> UploadImageAsync(Stream imageStream, string fileName, int maxWidth = 1920, int maxHeight = 1080, int quality = 85, string folder = "images")
        {
            throw new NotImplementedException("FileUploadService.UploadImageAsync is not implemented yet.");
        }

        public Task<string> UploadProfileImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("FileUploadService.UploadProfileImageAsync is not implemented yet.");
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            throw new NotImplementedException("FileUploadService.DeleteFileAsync is not implemented yet.");
        }

        public Task<bool> DeleteFilesAsync(List<string> fileUrls)
        {
            throw new NotImplementedException("FileUploadService.DeleteFilesAsync is not implemented yet.");
        }

        public bool IsValidFileType(string fileName, string[] allowedExtensions)
        {
            var extension = Path.GetExtension(fileName);
            return !string.IsNullOrWhiteSpace(extension) && allowedExtensions.Any(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsValidFileSize(long fileSize, int maxSizeInMB)
        {
            return fileSize <= maxSizeInMB * 1024 * 1024;
        }

        public string GenerateUniqueFileName(string originalFileName)
        {
            return $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        }
    }
}