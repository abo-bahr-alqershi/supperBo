using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using YemenBooking.Core.Interfaces.Events;
using YemenBooking.Core.Interfaces.Repositories;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Infrastructure.Repositories;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Infrastructure.UnitOfWork;
using YemenBooking.Core.Interfaces;
using FluentValidation;
using MediatR;

namespace YemenBooking.Api.Extensions
{
    /// <summary>
    /// امتدادات حقن التبعيات لمشروع YemenBooking
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// يضيف كافة التبعيات: مستودعات البيانات، خدمات البنية التحتية، ومعالجات أحداث المجال
        /// </summary>
        public static IServiceCollection AddYemenBookingServices(this IServiceCollection services)
        {
            // تسجيل وحدة العمل
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // تسجيل المستودعات
            RegisterRepositories(services);

            // تسجيل خدمات البنية التحتية
            RegisterInfrastructureServices(services);
            
            return services;
        }

        /// <summary>
        /// يبحث ويسجل جميع الأصناف المنتهية بـ Repository كواجهاتها المطابقة
        /// </summary>
        private static void RegisterRepositories(IServiceCollection services)
        {
            var repoAssembly = Assembly.GetAssembly(typeof(BookingRepository));
            var repoTypes = repoAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"));

            foreach (var impl in repoTypes)
            {
                var iface = impl.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + impl.Name);
                if (iface != null)
                {
                    services.AddScoped(iface, impl);
                }
            }
        }

        /// <summary>
        /// يبحث ويسجل جميع الأصناف المنتهية بـ Service كواجهاتها المطابقة
        /// </summary>
        private static void RegisterInfrastructureServices(IServiceCollection services)
        {
            var svcAssembly = Assembly.GetAssembly(typeof(FileStorageService));
            var svcTypes = svcAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));

            foreach (var impl in svcTypes)
            {
                var iface = impl.GetInterfaces()
                    .FirstOrDefault(i => i.Name == "I" + impl.Name);
                if (iface != null)
                {
                    services.AddScoped(iface, impl);
                }
            }
            services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
        }
    }
} 