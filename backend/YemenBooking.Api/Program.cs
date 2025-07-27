using YemenBooking.Api.Extensions;
using YemenBooking.Api.Services;
using YemenBooking.Application.Handlers.Commands.PropertyImages;
using YemenBooking.Application.Interfaces.Services;
using YemenBooking.Application.Mappings;
using YemenBooking.Core.Interfaces.Services;
using YemenBooking.Core.Settings;
using YemenBooking.Infrastructure;
using YemenBooking.Infrastructure.Data;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Infrastructure.Dapper;
using YemenBooking.Infrastructure.Migrations;
using YemenBooking.Infrastructure.Services;
using YemenBooking.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using AdvancedIndexingSystem.Core.Interfaces;
using AdvancedIndexingSystem.Core.Models;
using AdvancedIndexingSystem.Core.Services;

var builder = WebApplication.CreateBuilder(args);
// Register WebSocket manager and service for chat
builder.Services.AddSingleton<YemenBooking.Infrastructure.Services.WebSocketConnectionManager>();
builder.Services.AddScoped<YemenBooking.Core.Interfaces.Services.IWebSocketService, YemenBooking.Infrastructure.Services.WebSocketService>();

// إضافة خدمات Dapper
builder.Services.AddDapperRepository(builder.Configuration);

// Add services to the container.
// Configuring Swagger/OpenAPI with JWT security
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "YemenBooking API",
        Version = "v1",
        Description = "وثائق واجهة برمجة تطبيقات YemenBooking"
    });
    // تعريف أمان JWT
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "أدخل 'Bearer ' متبوعًا برمز JWT"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    // تضمين تعليقات XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// إضافة MediatR مع معالجات الأوامر
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreatePropertyImageCommandHandler).Assembly);
});

// إضافة AutoMapper وتهيئة ملفات Mapping يدويًا لتجنب خطأ MissingMethodException
builder.Services.AddAutoMapper(
    cfg => cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies()),
    AppDomain.CurrentDomain.GetAssemblies());

// إضافة خدمات المشروع
builder.Services.AddYemenBookingServices();
// إضافة التخزين المؤقت في الذاكرة لحفظ الفهارس
builder.Services.AddMemoryCache();

// إضافة دعم Controllers لربط المتحكمات مع دعم تحويل الـ enum كسلاسل نصية
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// إضافة سياسة CORS للسماح بالاتصالات من الواجهة الأمامية
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://localhost:5000", // Your actual frontend URL
            "http://localhost:5173", 
            "https://localhost:5173"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true)
        .WithExposedHeaders("*")
    );
});

// تسجيل إعدادات JWT من ملفات التكوين
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// إعداد المصادقة باستخدام JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    options.RequireHttpsMetadata = false; // Changed to false for development
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuerSigningKey = true
    };
});

// إضافة التفويض
builder.Services.AddAuthorization();

// إعداد DbContext لاستخدام SQLite
builder.Services.AddDbContext<YemenBookingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.ModelValidationKeyDefaultValueWarning))
);

// إضافة HttpContextAccessor لاستخدامه في CurrentUserService
builder.Services.AddHttpContextAccessor();

// إضافة HttpClient للخدمات التي تحتاجه
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>();
builder.Services.AddHttpClient<IPaymentGatewayService, PaymentGatewayService>();

// تسجيل خدمات الفهرسة المتقدمة
builder.Services.Configure<IndexingSettings>(builder.Configuration.GetSection("IndexingSettings"));

// تسجيل خدمات مكتبة الفهرسة المتقدمة
builder.Services.AddSingleton<IAdvancedIndex<PropertyIndexItem>, AdvancedIndexService<PropertyIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "property_index",
        IndexName = "PropertyIndex",
        ArabicName = "فهرس العقارات",
        Description = "فهرس البحث السريع في العقارات",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.High
    };
    return new AdvancedIndexService<PropertyIndexItem>(config);
});

builder.Services.AddSingleton<IAdvancedIndex<UnitIndexItem>, AdvancedIndexService<UnitIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "unit_index", 
        IndexName = "UnitIndex",
        ArabicName = "فهرس الوحدات",
        Description = "فهرس البحث السريع في الوحدات مع الحقول الديناميكية",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.High
    };
    return new AdvancedIndexService<UnitIndexItem>(config);
});

builder.Services.AddSingleton<IAdvancedIndex<PricingIndexItem>, AdvancedIndexService<PricingIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "pricing_index",
        IndexName = "PricingIndex", 
        ArabicName = "فهرس التسعير",
        Description = "فهرس قواعد التسعير للوحدات",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.Medium
    };
    return new AdvancedIndexService<PricingIndexItem>(config);
});

builder.Services.AddSingleton<IAdvancedIndex<AvailabilityIndexItem>, AdvancedIndexService<AvailabilityIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "availability_index",
        IndexName = "AvailabilityIndex",
        ArabicName = "فهرس الإتاحة", 
        Description = "فهرس إتاحة الوحدات",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.Medium
    };
    return new AdvancedIndexService<AvailabilityIndexItem>(config);
});

builder.Services.AddSingleton<IAdvancedIndex<CityIndexItem>, AdvancedIndexService<CityIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "city_index",
        IndexName = "CityIndex",
        ArabicName = "فهرس المدن",
        Description = "فهرس المدن والعقارات المرتبطة بها",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.Low
    };
    return new AdvancedIndexService<CityIndexItem>(config);
});

builder.Services.AddSingleton<IAdvancedIndex<AmenityIndexItem>, AdvancedIndexService<AmenityIndexItem>>(provider =>
{
    var config = new IndexConfiguration
    {
        IndexId = "amenity_index",
        IndexName = "AmenityIndex",
        ArabicName = "فهرس المرافق",
        Description = "فهرس المرافق والعقارات المرتبطة بها",
        IndexType = IndexType.Hash,
        Priority = IndexPriority.Low
    };
    return new AdvancedIndexService<AmenityIndexItem>(config);
});

// تسجيل خدمة الفهرسة المخصصة لـ YemenBooking
builder.Services.AddSingleton<IYemenBookingIndexService, YemenBookingIndexService>();

// تسجيل خدمة الفهرسة للتطبيق
builder.Services.AddScoped<IIndexingService, IndexingService>();

// تسجيل خدمة EventPublisher
builder.Services.AddScoped<IEventPublisher, EventPublisherService>();
builder.Services.AddScoped<YemenBooking.Application.Interfaces.Services.ICitySettingsService, YemenBooking.Infrastructure.Services.CitySettingsService>();
builder.Services.AddScoped<DataSeedingService>();

var app = builder.Build();

// التأكد من وجود الإجراءات المخزنة عند بدء التطبيق
using (var scope = app.Services.CreateScope())
{
    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
    connection.Open();
    StoredProceduresInitializer.EnsureAdvancedSearchProc(connection);
}

// استخدام امتداد لتكوين كافة middleware الخاصة بالتطبيق
app.UseYemenBookingMiddlewares();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<YemenBookingDbContext>();
    context.Database.Migrate();
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
    seeder.SeedAsync().GetAwaiter().GetResult();
    
    // تهيئة خدمة الفهرسة
    try
    {
        var indexingService = scope.ServiceProvider.GetRequiredService<IIndexingService>();
        var initResult = indexingService.InitializeAsync().GetAwaiter().GetResult();
        if (initResult)
        {
            Console.WriteLine("✅ تم تهيئة خدمة الفهرسة بنجاح");
        }
        else
        {
            Console.WriteLine("⚠️ فشل في تهيئة خدمة الفهرسة");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ خطأ في تهيئة خدمة الفهرسة: {ex.Message}");
    }
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}