using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace YemenBooking.Api.Extensions
{
    /// <summary>
    /// امتدادات لتكوين الmiddleware الخاصة بتطبيق YemenBooking
    /// Extensions for configuring YemenBooking middleware
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// يهيئ جميع middleware: إعادة التوجيه لـHTTPS، المصادقة، التفويض، OpenAPI وربط المتحكمات
        /// Configures middleware: HTTPS redirection, authentication, authorization, OpenAPI and controllers
        /// </summary>
        public static WebApplication UseYemenBookingMiddlewares(this WebApplication app)
        {
            // منع الوصول المباشر إلى مرفقات الشات
            app.Use(async (context, next) =>
                {
                    if (context.Request.Path.StartsWithSegments("/uploads/ChatAttachments"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }
                    await next();
                });

            // خدمة ملفات الستاتيك: مسار wwwroot الافتراضي
            app.UseStaticFiles();
            // خدمة رفع الصور من مجلد Uploads
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsRoot),
                RequestPath = "/uploads"
            });

            // إعادة التوجيه إلى HTTPS (تجاوز في بيئة التطوير)
            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            // تفعيل نظام التوجيه
            app.UseRouting();
            // تطبيق سياسة CORS قبل المصادقة
            app.UseCors("AllowFrontend");
            // المصادقة باستخدام JWT
            app.UseAuthentication();
            // التفويض
            app.UseAuthorization();

            // تفعيل Swagger UI في بيئة التطوير
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "YemenBooking API V1");
                    c.RoutePrefix = string.Empty; // الواجهة عند الجذر
                });
            }

            // ربط المتحكمات بنظام التوجيه
            app.MapControllers();
            return app;
        }
    }
}