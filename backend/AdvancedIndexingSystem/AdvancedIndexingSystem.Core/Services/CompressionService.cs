using System.IO.Compression;
using System.Text;
using System.Text.Json;
using AdvancedIndexingSystem.Core.Models;

namespace AdvancedIndexingSystem.Core.Services;

/// <summary>
/// خدمة الضغط والفتح باستخدام GZIP
/// GZIP Compression and Decompression Service
/// </summary>
public class CompressionService
{
    private readonly CompressionLevel _defaultCompressionLevel;
    private readonly Encoding _encoding;

    /// <summary>
    /// إنشاء خدمة ضغط جديدة
    /// Create new compression service
    /// </summary>
    /// <param name="compressionLevel">مستوى الضغط - Compression level</param>
    /// <param name="encoding">ترميز النص - Text encoding</param>
    public CompressionService(CompressionLevel compressionLevel = CompressionLevel.Optimal, Encoding? encoding = null)
    {
        _defaultCompressionLevel = compressionLevel;
        _encoding = encoding ?? Encoding.UTF8;
    }

    #region String Compression Methods

    /// <summary>
    /// ضغط النص باستخدام GZIP
    /// Compress text using GZIP
    /// </summary>
    /// <param name="text">النص المراد ضغطه - Text to compress</param>
    /// <returns>البيانات المضغوطة - Compressed data</returns>
    public async Task<byte[]> CompressStringAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<byte>();

        var textBytes = _encoding.GetBytes(text);
        return await CompressBytesAsync(textBytes);
    }

    /// <summary>
    /// فتح النص المضغوط
    /// Decompress compressed text
    /// </summary>
    /// <param name="compressedData">البيانات المضغوطة - Compressed data</param>
    /// <returns>النص الأصلي - Original text</returns>
    public async Task<string> DecompressStringAsync(byte[] compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
            return string.Empty;

        var decompressedBytes = await DecompressBytesAsync(compressedData);
        return _encoding.GetString(decompressedBytes);
    }

    #endregion

    #region Byte Array Compression Methods

    /// <summary>
    /// ضغط البيانات الثنائية
    /// Compress binary data
    /// </summary>
    /// <param name="data">البيانات المراد ضغطها - Data to compress</param>
    /// <param name="compressionLevel">مستوى الضغط - Compression level</param>
    /// <returns>البيانات المضغوطة - Compressed data</returns>
    public async Task<byte[]> CompressBytesAsync(byte[] data, CompressionLevel? compressionLevel = null)
    {
        if (data == null || data.Length == 0)
            return Array.Empty<byte>();

        using var outputStream = new MemoryStream();
        using (var gzipStream = new GZipStream(outputStream, compressionLevel ?? _defaultCompressionLevel))
        {
            await gzipStream.WriteAsync(data, 0, data.Length);
        }

        return outputStream.ToArray();
    }

    /// <summary>
    /// فتح البيانات المضغوطة
    /// Decompress compressed data
    /// </summary>
    /// <param name="compressedData">البيانات المضغوطة - Compressed data</param>
    /// <returns>البيانات الأصلية - Original data</returns>
    public async Task<byte[]> DecompressBytesAsync(byte[] compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
            return Array.Empty<byte>();

        using var inputStream = new MemoryStream(compressedData);
        using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
        using var outputStream = new MemoryStream();

        await gzipStream.CopyToAsync(outputStream);
        return outputStream.ToArray();
    }

    #endregion

    #region File Compression Methods

    /// <summary>
    /// ضغط ملف
    /// Compress file
    /// </summary>
    /// <param name="inputFilePath">مسار الملف الأصلي - Input file path</param>
    /// <param name="outputFilePath">مسار الملف المضغوط - Output compressed file path</param>
    /// <param name="compressionLevel">مستوى الضغط - Compression level</param>
    /// <returns>معلومات الضغط - Compression info</returns>
    public async Task<CompressionResult> CompressFileAsync(string inputFilePath, string outputFilePath, CompressionLevel? compressionLevel = null)
    {
        var startTime = DateTime.UtcNow;
        var compressionResult = new CompressionResult
        {
            InputFilePath = inputFilePath,
            OutputFilePath = outputFilePath,
            CompressionLevel = compressionLevel ?? _defaultCompressionLevel
        };

        try
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException($"Input file not found: {inputFilePath}");

            var inputFileInfo = new FileInfo(inputFilePath);
            compressionResult.OriginalSize = inputFileInfo.Length;

            // Ensure output directory exists
            var outputDirectory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (var gzipStream = new GZipStream(outputStream, compressionResult.CompressionLevel))
            {
                await inputStream.CopyToAsync(gzipStream);
            }

            var outputFileInfo = new FileInfo(outputFilePath);
            compressionResult.CompressedSize = outputFileInfo.Length;
            compressionResult.CompressionRatio = compressionResult.OriginalSize > 0 
                ? (double)compressionResult.CompressedSize / compressionResult.OriginalSize 
                : 0.0;
            compressionResult.SpaceSaved = compressionResult.OriginalSize - compressionResult.CompressedSize;
            compressionResult.CompressionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            compressionResult.IsSuccessful = true;

            return compressionResult;
        }
        catch (Exception ex)
        {
            compressionResult.IsSuccessful = false;
            compressionResult.ErrorMessage = ex.Message;
            compressionResult.CompressionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            return compressionResult;
        }
    }

    /// <summary>
    /// فتح ملف مضغوط
    /// Decompress file
    /// </summary>
    /// <param name="compressedFilePath">مسار الملف المضغوط - Compressed file path</param>
    /// <param name="outputFilePath">مسار الملف المفتوح - Output decompressed file path</param>
    /// <returns>معلومات فتح الضغط - Decompression info</returns>
    public async Task<DecompressionResult> DecompressFileAsync(string compressedFilePath, string outputFilePath)
    {
        var startTime = DateTime.UtcNow;
        var decompressionResult = new DecompressionResult
        {
            CompressedFilePath = compressedFilePath,
            OutputFilePath = outputFilePath
        };

        try
        {
            if (!File.Exists(compressedFilePath))
                throw new FileNotFoundException($"Compressed file not found: {compressedFilePath}");

            var compressedFileInfo = new FileInfo(compressedFilePath);
            decompressionResult.CompressedSize = compressedFileInfo.Length;

            // Ensure output directory exists
            var outputDirectory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var inputStream = new FileStream(compressedFilePath, FileMode.Open, FileAccess.Read))
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                await gzipStream.CopyToAsync(outputStream);
            }

            var outputFileInfo = new FileInfo(outputFilePath);
            decompressionResult.DecompressedSize = outputFileInfo.Length;
            decompressionResult.DecompressionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            decompressionResult.IsSuccessful = true;

            return decompressionResult;
        }
        catch (Exception ex)
        {
            decompressionResult.IsSuccessful = false;
            decompressionResult.ErrorMessage = ex.Message;
            decompressionResult.DecompressionTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds;
            return decompressionResult;
        }
    }

    #endregion

    #region JSON Compression Methods

    /// <summary>
    /// ضغط كائن JSON
    /// Compress JSON object
    /// </summary>
    /// <typeparam name="T">نوع الكائن - Object type</typeparam>
    /// <param name="obj">الكائن المراد ضغطه - Object to compress</param>
    /// <param name="jsonOptions">خيارات JSON - JSON options</param>
    /// <returns>البيانات المضغوطة - Compressed data</returns>
    public async Task<byte[]> CompressJsonAsync<T>(T obj, JsonSerializerOptions? jsonOptions = null)
    {
        if (obj == null)
            return Array.Empty<byte>();

        var jsonString = JsonSerializer.Serialize(obj, jsonOptions ?? GetDefaultJsonOptions());
        return await CompressStringAsync(jsonString);
    }

    /// <summary>
    /// فتح كائن JSON مضغوط
    /// Decompress compressed JSON object
    /// </summary>
    /// <typeparam name="T">نوع الكائن - Object type</typeparam>
    /// <param name="compressedData">البيانات المضغوطة - Compressed data</param>
    /// <param name="jsonOptions">خيارات JSON - JSON options</param>
    /// <returns>الكائن الأصلي - Original object</returns>
    public async Task<T?> DecompressJsonAsync<T>(byte[] compressedData, JsonSerializerOptions? jsonOptions = null)
    {
        if (compressedData == null || compressedData.Length == 0)
            return default(T);

        var jsonString = await DecompressStringAsync(compressedData);
        if (string.IsNullOrEmpty(jsonString))
            return default(T);

        return JsonSerializer.Deserialize<T>(jsonString, jsonOptions ?? GetDefaultJsonOptions());
    }

    #endregion

    #region Index-Specific Compression Methods

    /// <summary>
    /// ضغط تكوين الفهرس
    /// Compress index configuration
    /// </summary>
    /// <param name="configuration">تكوين الفهرس - Index configuration</param>
    /// <returns>البيانات المضغوطة - Compressed data</returns>
    public async Task<byte[]> CompressIndexConfigurationAsync(IndexConfiguration configuration)
    {
        return await CompressJsonAsync(configuration);
    }

    /// <summary>
    /// فتح تكوين الفهرس المضغوط
    /// Decompress compressed index configuration
    /// </summary>
    /// <param name="compressedData">البيانات المضغوطة - Compressed data</param>
    /// <returns>تكوين الفهرس - Index configuration</returns>
    public async Task<IndexConfiguration?> DecompressIndexConfigurationAsync(byte[] compressedData)
    {
        return await DecompressJsonAsync<IndexConfiguration>(compressedData);
    }

    /// <summary>
    /// ضغط نتائج البحث
    /// Compress search results
    /// </summary>
    /// <typeparam name="T">نوع البيانات - Data type</typeparam>
    /// <param name="searchResult">نتائج البحث - Search results</param>
    /// <returns>البيانات المضغوطة - Compressed data</returns>
    public async Task<byte[]> CompressSearchResultAsync<T>(SearchResult<T> searchResult)
    {
        return await CompressJsonAsync(searchResult);
    }

    /// <summary>
    /// فتح نتائج البحث المضغوطة
    /// Decompress compressed search results
    /// </summary>
    /// <typeparam name="T">نوع البيانات - Data type</typeparam>
    /// <param name="compressedData">البيانات المضغوطة - Compressed data</param>
    /// <returns>نتائج البحث - Search results</returns>
    public async Task<SearchResult<T>?> DecompressSearchResultAsync<T>(byte[] compressedData)
    {
        return await DecompressJsonAsync<SearchResult<T>>(compressedData);
    }

    #endregion

    #region Batch Compression Methods

    /// <summary>
    /// ضغط متعدد الملفات
    /// Compress multiple files
    /// </summary>
    /// <param name="filePaths">مسارات الملفات - File paths</param>
    /// <param name="outputDirectory">مجلد الإخراج - Output directory</param>
    /// <param name="compressionLevel">مستوى الضغط - Compression level</param>
    /// <returns>نتائج الضغط - Compression results</returns>
    public async Task<List<CompressionResult>> CompressFilesAsync(
        IEnumerable<string> filePaths, 
        string outputDirectory, 
        CompressionLevel? compressionLevel = null)
    {
        var results = new List<CompressionResult>();
        var tasks = new List<Task<CompressionResult>>();

        foreach (var filePath in filePaths)
        {
            var fileName = Path.GetFileName(filePath);
            var outputPath = Path.Combine(outputDirectory, $"{fileName}.gz");
            tasks.Add(CompressFileAsync(filePath, outputPath, compressionLevel));
        }

        results.AddRange(await Task.WhenAll(tasks));
        return results;
    }

    /// <summary>
    /// فتح متعدد الملفات
    /// Decompress multiple files
    /// </summary>
    /// <param name="compressedFilePaths">مسارات الملفات المضغوطة - Compressed file paths</param>
    /// <param name="outputDirectory">مجلد الإخراج - Output directory</param>
    /// <returns>نتائج فتح الضغط - Decompression results</returns>
    public async Task<List<DecompressionResult>> DecompressFilesAsync(
        IEnumerable<string> compressedFilePaths, 
        string outputDirectory)
    {
        var results = new List<DecompressionResult>();
        var tasks = new List<Task<DecompressionResult>>();

        foreach (var filePath in compressedFilePaths)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }
            var outputPath = Path.Combine(outputDirectory, fileName);
            tasks.Add(DecompressFileAsync(filePath, outputPath));
        }

        results.AddRange(await Task.WhenAll(tasks));
        return results;
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// حساب نسبة الضغط
    /// Calculate compression ratio
    /// </summary>
    /// <param name="originalSize">الحجم الأصلي - Original size</param>
    /// <param name="compressedSize">الحجم المضغوط - Compressed size</param>
    /// <returns>نسبة الضغط - Compression ratio</returns>
    public static double CalculateCompressionRatio(long originalSize, long compressedSize)
    {
        if (originalSize == 0) return 0.0;
        return (double)compressedSize / originalSize;
    }

    /// <summary>
    /// حساب نسبة التوفير
    /// Calculate space savings percentage
    /// </summary>
    /// <param name="originalSize">الحجم الأصلي - Original size</param>
    /// <param name="compressedSize">الحجم المضغوط - Compressed size</param>
    /// <returns>نسبة التوفير - Space savings percentage</returns>
    public static double CalculateSpaceSavingsPercentage(long originalSize, long compressedSize)
    {
        if (originalSize == 0) return 0.0;
        return ((double)(originalSize - compressedSize) / originalSize) * 100.0;
    }

    /// <summary>
    /// التحقق من صحة الملف المضغوط
    /// Validate compressed file
    /// </summary>
    /// <param name="compressedFilePath">مسار الملف المضغوط - Compressed file path</param>
    /// <returns>هل الملف صحيح - Is file valid</returns>
    public async Task<bool> ValidateCompressedFileAsync(string compressedFilePath)
    {
        try
        {
            if (!File.Exists(compressedFilePath))
                return false;

            using var fileStream = new FileStream(compressedFilePath, FileMode.Open, FileAccess.Read);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
            using var memoryStream = new MemoryStream();

            await gzipStream.CopyToAsync(memoryStream);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// الحصول على خيارات JSON الافتراضية
    /// Get default JSON options
    /// </summary>
    /// <returns>خيارات JSON - JSON options</returns>
    private static JsonSerializerOptions GetDefaultJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    #endregion
}

/// <summary>
/// نتيجة عملية الضغط
/// Compression operation result
/// </summary>
public class CompressionResult
{
    /// <summary>
    /// مسار الملف الأصلي
    /// Input file path
    /// </summary>
    public string InputFilePath { get; set; } = string.Empty;

    /// <summary>
    /// مسار الملف المضغوط
    /// Output compressed file path
    /// </summary>
    public string OutputFilePath { get; set; } = string.Empty;

    /// <summary>
    /// الحجم الأصلي بالبايت
    /// Original size in bytes
    /// </summary>
    public long OriginalSize { get; set; }

    /// <summary>
    /// الحجم المضغوط بالبايت
    /// Compressed size in bytes
    /// </summary>
    public long CompressedSize { get; set; }

    /// <summary>
    /// نسبة الضغط
    /// Compression ratio
    /// </summary>
    public double CompressionRatio { get; set; }

    /// <summary>
    /// المساحة الموفرة بالبايت
    /// Space saved in bytes
    /// </summary>
    public long SpaceSaved { get; set; }

    /// <summary>
    /// مستوى الضغط المستخدم
    /// Compression level used
    /// </summary>
    public CompressionLevel CompressionLevel { get; set; }

    /// <summary>
    /// وقت الضغط بالميلي ثانية
    /// Compression time in milliseconds
    /// </summary>
    public double CompressionTimeMs { get; set; }

    /// <summary>
    /// هل العملية ناجحة
    /// Is operation successful
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// رسالة الخطأ (في حالة الفشل)
    /// Error message (if failed)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// نسبة التوفير بالنسبة المئوية
    /// Space savings percentage
    /// </summary>
    public double SpaceSavingsPercentage => CompressionService.CalculateSpaceSavingsPercentage(OriginalSize, CompressedSize);
}

/// <summary>
/// نتيجة عملية فتح الضغط
/// Decompression operation result
/// </summary>
public class DecompressionResult
{
    /// <summary>
    /// مسار الملف المضغوط
    /// Compressed file path
    /// </summary>
    public string CompressedFilePath { get; set; } = string.Empty;

    /// <summary>
    /// مسار الملف المفتوح
    /// Output decompressed file path
    /// </summary>
    public string OutputFilePath { get; set; } = string.Empty;

    /// <summary>
    /// حجم الملف المضغوط بالبايت
    /// Compressed size in bytes
    /// </summary>
    public long CompressedSize { get; set; }

    /// <summary>
    /// حجم الملف المفتوح بالبايت
    /// Decompressed size in bytes
    /// </summary>
    public long DecompressedSize { get; set; }

    /// <summary>
    /// وقت فتح الضغط بالميلي ثانية
    /// Decompression time in milliseconds
    /// </summary>
    public double DecompressionTimeMs { get; set; }

    /// <summary>
    /// هل العملية ناجحة
    /// Is operation successful
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// رسالة الخطأ (في حالة الفشل)
    /// Error message (if failed)
    /// </summary>
    public string? ErrorMessage { get; set; }
}