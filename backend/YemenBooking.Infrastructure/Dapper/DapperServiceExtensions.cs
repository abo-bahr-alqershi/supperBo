using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;
using YemenBooking.Application.Interfaces.Repositories;
using YemenBooking.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;

namespace YemenBooking.Infrastructure.Dapper
{
    /// <summary>
    /// امتدادات لتسجيل خدمات Dapper في الحاوية
    /// Extension methods for registering Dapper services
    /// </summary>
    public static class DapperServiceExtensions
    {
        /// <summary>
        /// يضيف IDbConnection ومؤسسة البحث المتقدم لمعاملات Dapper
        /// Registers IDbConnection and advanced search repository for Dapper
        /// </summary>
        public static IServiceCollection AddDapperRepository(this IServiceCollection services, IConfiguration configuration)
        {
            // إعداد الاتصال بقاعدة البيانات
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddTransient<IDbConnection>(sp => new SqliteConnection(connectionString));

            // تسجيل مستودع البحث المتقدم
            services.AddTransient<IAdvancedPropertySearchRepository, AdvancedPropertySearchRepository>();

            return services;
        }
    }
} 