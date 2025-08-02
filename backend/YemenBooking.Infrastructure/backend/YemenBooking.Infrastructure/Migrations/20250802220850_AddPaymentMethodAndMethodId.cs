using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.backend.YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodAndMethodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "Payments");

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice_ExchangeRate",
                table: "Units",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Units",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price_ExchangeRate",
                table: "PropertyServices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraCost_ExchangeRate",
                table: "PropertyAmenities",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePricePerNight",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount_ExchangeRate",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "MethodId",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice_ExchangeRate",
                table: "BookingServices",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice_ExchangeRate",
                table: "Bookings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "HomeScreenTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Name of the template"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Description of the template"),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Version of the template"),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the template is default"),
                    PublishedAt = table.Column<DateTime>(type: "datetime", nullable: true, comment: "Publication date of the template"),
                    PublishedBy = table.Column<Guid>(type: "TEXT", nullable: true, comment: "Identifier of the user who published the template"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Target platform of the template (iOS, Android, All)"),
                    TargetAudience = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Target audience of the template (Guest, User, Premium)"),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON metadata for additional settings"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeScreenTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IconUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAvailableForClients = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresVerification = table.Column<bool>(type: "INTEGER", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SupportedCountries = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    MinAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    MaxAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    FeePercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    FixedFee = table.Column<decimal>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeScreenSections",
                columns: table => new
                {
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen template"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Name of the section"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Title of the section"),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Subtitle of the section"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the section in the template"),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Visibility status of the section"),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Background color of the section"),
                    BackgroundImage = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Background image URL for the section"),
                    Padding = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Padding of the section"),
                    Margin = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Margin of the section"),
                    MinHeight = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Minimum height of the section"),
                    MaxHeight = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Maximum height of the section"),
                    CustomStyles = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON for custom styles"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for section visibility"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeScreenSections", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_HomeScreenSections_HomeScreenTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "HomeScreenTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserHomeScreens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomizationData = table.Column<string>(type: "TEXT", nullable: false),
                    UserPreferences = table.Column<string>(type: "TEXT", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeviceInfo = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHomeScreens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHomeScreens_HomeScreenTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "HomeScreenTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHomeScreens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeScreenComponents",
                columns: table => new
                {
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen section"),
                    ComponentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Type of the component (Banner, Carousel, CategoryGrid, etc.)"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Name of the component"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the component in the section"),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Visibility status of the component"),
                    ColSpan = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 12, comment: "Column span of the component (1-12)"),
                    RowSpan = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1, comment: "Row span of the component"),
                    Alignment = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Alignment of the component (left, center, right)"),
                    CustomClasses = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Custom CSS classes for the component"),
                    AnimationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of animation for the component"),
                    AnimationDuration = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Duration of the animation in milliseconds"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for component visibility or behavior"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeScreenComponents", x => x.ComponentId);
                    table.ForeignKey(
                        name: "FK_HomeScreenComponents_HomeScreenSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "HomeScreenSections",
                        principalColumn: "SectionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentActions",
                columns: table => new
                {
                    ActionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen component"),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of action (Navigate, OpenModal, CallAPI, Share)"),
                    ActionTrigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Trigger of action (Click, LongPress, Swipe)"),
                    ActionTarget = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Target of action (Screen name, URL, API endpoint)"),
                    ActionParams = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON parameters for the action"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for the action"),
                    RequiresAuth = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Whether authentication is required for the action"),
                    AnimationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of animation for the action"),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Priority of the action"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentActions", x => x.ActionId);
                    table.ForeignKey(
                        name: "FK_ComponentActions_HomeScreenComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "HomeScreenComponents",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentDataSources",
                columns: table => new
                {
                    DataSourceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen component"),
                    SourceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of data source (Static, API, Database, Cache)"),
                    DataEndpoint = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Data endpoint URL or path"),
                    HttpMethod = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, comment: "HTTP method for the data request"),
                    Headers = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON headers for the request"),
                    QueryParams = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON query parameters for the request"),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON request body"),
                    DataMapping = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON field mapping"),
                    CacheKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Cache key for the data source"),
                    CacheDuration = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Cache duration in minutes"),
                    RefreshTrigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Trigger for cache refresh (OnLoad, OnFocus, Manual, Timer)"),
                    RefreshInterval = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Refresh interval in seconds"),
                    ErrorHandling = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON error handling configuration"),
                    MockData = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON mock data for development"),
                    UseMockInDev = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Flag indicating mock data usage in development"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDataSources", x => x.DataSourceId);
                    table.ForeignKey(
                        name: "FK_ComponentDataSources_HomeScreenComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "HomeScreenComponents",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentProperties",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen component"),
                    PropertyKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Unique key for the property"),
                    PropertyName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Display name of the property"),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of the property (text, number, boolean, select, color, image)"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Current value of the property"),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Default value of the property"),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the property is required"),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON validation rules"),
                    Options = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON options for select type"),
                    HelpText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Help text for the property"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the property in the component"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentProperties", x => x.PropertyId);
                    table.ForeignKey(
                        name: "FK_ComponentProperties_HomeScreenComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "HomeScreenComponents",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponentStyles",
                columns: table => new
                {
                    StyleId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ComponentId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen component"),
                    StyleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Key of the style"),
                    StyleValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Value of the style"),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Unit of measurement (px, %, em, rem)"),
                    IsImportant = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the style is marked as important"),
                    MediaQuery = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Media query for responsive styles"),
                    State = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "State for which the style applies (hover, active, focus)"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Platform for which the style applies (iOS, Android, All)"),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentStyles", x => x.StyleId);
                    table.ForeignKey(
                        name: "FK_ComponentStyles_HomeScreenComponents_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "HomeScreenComponents",
                        principalColumn: "ComponentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7008), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7008) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7026), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7026) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7031), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7031) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7036), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7037) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7041), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7041) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7046), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7055) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7061), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7062) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7068), new DateTime(2025, 8, 2, 22, 8, 49, 132, DateTimeKind.Utc).AddTicks(7068) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(349), new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(350) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(370), new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(370) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(376), new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(376) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(382), new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(382) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(387), new DateTime(2025, 8, 2, 22, 8, 49, 91, DateTimeKind.Utc).AddTicks(387) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3185), new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3186) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3211), new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3212) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3217), new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3217) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3222), new DateTime(2025, 8, 2, 22, 8, 49, 89, DateTimeKind.Utc).AddTicks(3223) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9523), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9523), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9524) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9529), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9530), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9530) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "EmailVerifiedAt", "IsEmailVerified", "LastLoginDate", "ProfileImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9425), null, false, new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9427), "", new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9403) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "EmailVerifiedAt", "IsEmailVerified", "LastLoginDate", "ProfileImageUrl", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9459), null, false, new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9459), "", new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9454) });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_MethodId",
                table: "Payments",
                column: "MethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentActions_ComponentId",
                table: "ComponentActions",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDataSources_ComponentId",
                table: "ComponentDataSources",
                column: "ComponentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentProperties_ComponentId",
                table: "ComponentProperties",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentStyles_ComponentId",
                table: "ComponentStyles",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeScreenComponents_SectionId",
                table: "HomeScreenComponents",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeScreenSections_TemplateId",
                table: "HomeScreenSections",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHomeScreens_TemplateId",
                table: "UserHomeScreens",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHomeScreens_UserId",
                table: "UserHomeScreens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentMethods_MethodId",
                table: "Payments",
                column: "MethodId",
                principalTable: "PaymentMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_PaymentMethods_MethodId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "ComponentActions");

            migrationBuilder.DropTable(
                name: "ComponentDataSources");

            migrationBuilder.DropTable(
                name: "ComponentProperties");

            migrationBuilder.DropTable(
                name: "ComponentStyles");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "UserHomeScreens");

            migrationBuilder.DropTable(
                name: "HomeScreenComponents");

            migrationBuilder.DropTable(
                name: "HomeScreenSections");

            migrationBuilder.DropTable(
                name: "HomeScreenTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Payments_MethodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BasePrice_ExchangeRate",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Price_ExchangeRate",
                table: "PropertyServices");

            migrationBuilder.DropColumn(
                name: "ExtraCost_ExchangeRate",
                table: "PropertyAmenities");

            migrationBuilder.DropColumn(
                name: "BasePricePerNight",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Amount_ExchangeRate",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MethodId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TotalPrice_ExchangeRate",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "TotalPrice_ExchangeRate",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "Payments",
                type: "INTEGER",
                maxLength: 50,
                nullable: false,
                defaultValue: 0,
                comment: "طريقة الدفع");

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(6328), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(6330) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7712), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7713) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7733), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7734) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7748), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7748) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7763), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7763) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7775), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7775) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7789), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7790) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7803), new DateTime(2025, 7, 24, 5, 30, 49, 644, DateTimeKind.Utc).AddTicks(7803) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(2837), new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(2838) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3960), new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3960) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3974), new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3974) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3985), new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3985) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3995), new DateTime(2025, 7, 24, 5, 30, 49, 479, DateTimeKind.Utc).AddTicks(3996) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(8408), new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(8409) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9262), new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9262) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9287), new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9287) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9297), new DateTime(2025, 7, 24, 5, 30, 49, 466, DateTimeKind.Utc).AddTicks(9297) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(5342), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(5342), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(5343) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(6527), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(6527), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(6528) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 856, DateTimeKind.Utc).AddTicks(6986), new DateTime(2025, 7, 24, 5, 30, 49, 856, DateTimeKind.Utc).AddTicks(7851), new DateTime(2025, 7, 24, 5, 30, 49, 856, DateTimeKind.Utc).AddTicks(4742) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(3117), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(3118), new DateTime(2025, 7, 24, 5, 30, 49, 857, DateTimeKind.Utc).AddTicks(3106) });
        }
    }
}
