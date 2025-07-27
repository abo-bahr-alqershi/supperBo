using Dapper;
using System.Data;

namespace YemenBooking.Infrastructure.Dapper
{
    /// <summary>
    /// يقوم بالتأكد من وجود الإجراءات المخزنة في قاعدة البيانات، وإنشاؤها إذا لم تكن موجودة
    /// Ensures stored procedures are created if missing
    /// </summary>
    public static class StoredProceduresInitializer
    {
        /// <summary>
        /// يتأكد من وجود sp_AdvancedPropertySearch في قاعدة البيانات
        /// Creates sp_AdvancedPropertySearch if not exists
        /// </summary>
        public static void EnsureAdvancedSearchProc(IDbConnection connection)
        {
            if(connection.GetType().Name.Contains("Sqlite",System.StringComparison.OrdinalIgnoreCase))
                return;
            const string procName = "sp_AdvancedPropertySearch";
            // التحقق من وجود الإجراء
            var exists = connection.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(@Name) AND type = 'P'", new { Name = procName });
            if (exists == 0)
            {
                // تعريف الإجراء المخزن
                var sql = @"
CREATE PROCEDURE sp_AdvancedPropertySearch
    @PropertyTypeId UNIQUEIDENTIFIER = NULL,
    @FromDate DATETIME2 = NULL,
    @ToDate DATETIME2 = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @Currency NVARCHAR(10) = NULL,
    @PrimaryFieldFilters dbo.JsonFilters READONLY,
    @FieldFilters dbo.JsonFilters READONLY,
    @UnitTypeIds dbo.GuidList READONLY,
    @AmenityIds dbo.GuidList READONLY,
    @ServiceIds dbo.GuidList READONLY,
    @SortBy NVARCHAR(50),
    @IsAscending BIT = 0,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    -- 1. حساب إجمالي عدد الكيانات المطابقة للفلترة
    SELECT COUNT(DISTINCT p.Id)
    FROM Properties p
    LEFT JOIN Units u ON u.PropertyId = p.Id
    LEFT JOIN PricingRules pr ON pr.UnitId = u.Id AND pr.StartDate <= @FromDate AND pr.EndDate >= @ToDate AND pr.Currency = @Currency
    WHERE p.IsDeleted = 0
      AND (@PropertyTypeId IS NULL OR p.TypeId = @PropertyTypeId)
      AND (@MinPrice IS NULL OR u.BasePrice >= @MinPrice)
      AND (@MaxPrice IS NULL OR u.BasePrice <= @MaxPrice);

    -- 2. جلب الصفحة المطلوبة من النتائج
    SELECT
        p.Id,
        img.Url AS MainImageUrl,
        p.Name,
        ISNULL(AVG((r.Cleanliness + r.Service + r.Location + r.Value)/4.0), 0) AS AverageRating,
        0 AS IsFavorite,
        COUNT(r.Id) AS ReviewsCount,
        p.StarRating,
        u.BasePrice AS BasePrice,
        CASE WHEN pr.PriceAmount < u.BasePrice THEN pr.PriceAmount ELSE u.BasePrice END AS EffectivePrice,
        p.Latitude, p.Longitude
    FROM Properties p
    LEFT JOIN PropertyImages img ON img.PropertyId = p.Id AND img.IsMainImage = 1 AND img.IsDeleted = 0
    LEFT JOIN Units u ON u.PropertyId = p.Id
    LEFT JOIN PricingRules pr ON pr.UnitId = u.Id AND pr.StartDate <= @FromDate AND pr.EndDate >= @ToDate AND pr.Currency = @Currency
    LEFT JOIN Reviews r ON r.BookingId IN (SELECT BookingId FROM Bookings WHERE UnitId = u.Id)
    WHERE p.IsDeleted = 0
      AND (@PropertyTypeId IS NULL OR p.TypeId = @PropertyTypeId)
      AND (@MinPrice IS NULL OR u.BasePrice >= @MinPrice)
      AND (@MaxPrice IS NULL OR u.BasePrice <= @MaxPrice)
    GROUP BY p.Id, img.Url, p.Name, p.StarRating, u.BasePrice, pr.PriceAmount, p.Latitude, p.Longitude
    ORDER BY 
        CASE WHEN @SortBy = 'name' THEN p.Name END ASC,
        CASE WHEN @SortBy = 'price' THEN EffectivePrice END 
        OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;
END";

                // تنفيذ إنشاء الإجراء
                connection.Execute(sql);
            }
        }
    }
} 