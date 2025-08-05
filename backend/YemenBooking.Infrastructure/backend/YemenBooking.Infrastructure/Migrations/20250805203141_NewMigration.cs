using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemenBooking.Infrastructure.backend.YemenBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentActions");

            migrationBuilder.DropTable(
                name: "ComponentDataSources");

            migrationBuilder.DropTable(
                name: "ComponentProperties");

            migrationBuilder.DropTable(
                name: "ComponentStyles");

            migrationBuilder.DropTable(
                name: "UserHomeScreens");

            migrationBuilder.DropTable(
                name: "HomeScreenComponents");

            migrationBuilder.DropTable(
                name: "HomeScreenSections");

            migrationBuilder.DropTable(
                name: "HomeScreenTemplates");

            migrationBuilder.CreateTable(
                name: "CityDestinationSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CountryAr = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DescriptionAr = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AdditionalImages = table.Column<string>(type: "json", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: false),
                    PropertyCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AveragePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", nullable: false),
                    ReviewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPopular = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Highlights = table.Column<string>(type: "json", nullable: false),
                    HighlightsAr = table.Column<string>(type: "json", nullable: false),
                    WeatherData = table.Column<string>(type: "json", nullable: false),
                    AttractionsData = table.Column<string>(type: "json", nullable: false),
                    Metadata = table.Column<string>(type: "json", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityDestinationSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicHomeConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GlobalSettings = table.Column<string>(type: "json", nullable: false),
                    ThemeSettings = table.Column<string>(type: "json", nullable: false),
                    LayoutSettings = table.Column<string>(type: "json", nullable: false),
                    CacheSettings = table.Column<string>(type: "json", nullable: false),
                    AnalyticsSettings = table.Column<string>(type: "json", nullable: false),
                    EnabledFeatures = table.Column<string>(type: "json", nullable: false),
                    ExperimentalFeatures = table.Column<string>(type: "json", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicHomeConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicHomeSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    TitleAr = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SubtitleAr = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SectionConfig = table.Column<string>(type: "json", nullable: false),
                    Metadata = table.Column<string>(type: "json", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TargetAudience = table.Column<string>(type: "json", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicHomeSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SponsoredAdSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PropertyIds = table.Column<string>(type: "json", nullable: false),
                    CustomImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TextColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Styling = table.Column<string>(type: "json", nullable: false),
                    CtaText = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CtaAction = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CtaData = table.Column<string>(type: "json", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetingData = table.Column<string>(type: "json", nullable: false),
                    AnalyticsData = table.Column<string>(type: "json", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ImpressionCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ClickCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ConversionRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsoredAdSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSectionContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ContentData = table.Column<string>(type: "json", nullable: false),
                    Metadata = table.Column<string>(type: "json", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSectionContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSectionContent_DynamicHomeSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "DynamicHomeSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4509), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4509) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4528), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4529) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4541), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4541) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4552), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4552) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4562), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4562) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4573), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4573) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4583), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4584) });

            migrationBuilder.UpdateData(
                table: "Amenities",
                keyColumn: "AmenityId",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4594), new DateTime(2025, 8, 5, 20, 31, 39, 765, DateTimeKind.Utc).AddTicks(4595) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2153), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2153) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2187), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2187) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2199), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2199) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2222), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2222) });

            migrationBuilder.UpdateData(
                table: "PropertyTypes",
                keyColumn: "TypeId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2233), new DateTime(2025, 8, 5, 20, 31, 39, 695, DateTimeKind.Utc).AddTicks(2233) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3458), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3458) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3483), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3483) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3494), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3494) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3504), new DateTime(2025, 8, 5, 20, 31, 39, 693, DateTimeKind.Utc).AddTicks(3505) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5740), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5741), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5741) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                columns: new[] { "AssignedAt", "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5745) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5587), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5588), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5580) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5630), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5631), new DateTime(2025, 8, 5, 20, 31, 39, 810, DateTimeKind.Utc).AddTicks(5628) });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSectionContent_SectionId",
                table: "DynamicSectionContent",
                column: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CityDestinationSections");

            migrationBuilder.DropTable(
                name: "DynamicHomeConfigs");

            migrationBuilder.DropTable(
                name: "DynamicSectionContent");

            migrationBuilder.DropTable(
                name: "SponsoredAdSections");

            migrationBuilder.DropTable(
                name: "DynamicHomeSections");

            migrationBuilder.CreateTable(
                name: "HomeScreenTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Description of the template"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the template is default"),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    MetaData = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON metadata for additional settings"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Name of the template"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Target platform of the template (iOS, Android, All)"),
                    PublishedAt = table.Column<DateTime>(type: "datetime", nullable: true, comment: "Publication date of the template"),
                    PublishedBy = table.Column<Guid>(type: "TEXT", nullable: true, comment: "Identifier of the user who published the template"),
                    TargetAudience = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Target audience of the template (Guest, User, Premium)"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Version of the template")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeScreenTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "HomeScreenSections",
                columns: table => new
                {
                    SectionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false, comment: "Identifier of the home screen template"),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Background color of the section"),
                    BackgroundImage = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Background image URL for the section"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for section visibility"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CustomStyles = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON for custom styles"),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Visibility status of the section"),
                    Margin = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Margin of the section"),
                    MaxHeight = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Maximum height of the section"),
                    MinHeight = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Minimum height of the section"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Name of the section"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the section in the template"),
                    Padding = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Padding of the section"),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Subtitle of the section"),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Title of the section"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    TemplateId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CustomizationData = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeviceInfo = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastViewedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserPreferences = table.Column<string>(type: "TEXT", nullable: false)
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
                    Alignment = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Alignment of the component (left, center, right)"),
                    AnimationDuration = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Duration of the animation in milliseconds"),
                    AnimationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of animation for the component"),
                    ColSpan = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 12, comment: "Column span of the component (1-12)"),
                    ComponentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Type of the component (Banner, Carousel, CategoryGrid, etc.)"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for component visibility or behavior"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    CustomClasses = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Custom CSS classes for the component"),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true, comment: "Visibility status of the component"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Name of the component"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the component in the section"),
                    RowSpan = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1, comment: "Row span of the component"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    ActionParams = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON parameters for the action"),
                    ActionTarget = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Target of action (Screen name, URL, API endpoint)"),
                    ActionTrigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Trigger of action (Click, LongPress, Swipe)"),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of action (Navigate, OpenModal, CallAPI, Share)"),
                    AnimationType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of animation for the action"),
                    Conditions = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON conditions for the action"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Priority of the action"),
                    RequiresAuth = table.Column<bool>(type: "INTEGER", nullable: false, comment: "Whether authentication is required for the action"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
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
                    CacheDuration = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Cache duration in minutes"),
                    CacheKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Cache key for the data source"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DataEndpoint = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Data endpoint URL or path"),
                    DataMapping = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON field mapping"),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ErrorHandling = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON error handling configuration"),
                    Headers = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON headers for the request"),
                    HttpMethod = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false, comment: "HTTP method for the data request"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    MockData = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON mock data for development"),
                    QueryParams = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON query parameters for the request"),
                    RefreshInterval = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Refresh interval in seconds"),
                    RefreshTrigger = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Trigger for cache refresh (OnLoad, OnFocus, Manual, Timer)"),
                    RequestBody = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON request body"),
                    SourceType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of data source (Static, API, Database, Cache)"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    UseMockInDev = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Flag indicating mock data usage in development")
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DefaultValue = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Default value of the property"),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    HelpText = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Help text for the property"),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the property is required"),
                    Options = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON options for select type"),
                    Order = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0, comment: "Order of the property in the component"),
                    PropertyKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Unique key for the property"),
                    PropertyName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Display name of the property"),
                    PropertyType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, comment: "Type of the property (text, number, boolean, select, color, image)"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    ValidationRules = table.Column<string>(type: "TEXT", nullable: false, comment: "JSON validation rules"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false, comment: "Current value of the property")
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
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsImportant = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false, comment: "Whether the style is marked as important"),
                    MediaQuery = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Media query for responsive styles"),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Platform for which the style applies (iOS, Android, All)"),
                    State = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "State for which the style applies (hover, active, focus)"),
                    StyleKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false, comment: "Key of the style"),
                    StyleValue = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false, comment: "Value of the style"),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false, comment: "Unit of measurement (px, %, em, rem)"),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "TEXT", nullable: true)
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
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9425), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9427), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9403) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                columns: new[] { "CreatedAt", "LastLoginDate", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9459), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9459), new DateTime(2025, 8, 2, 22, 8, 49, 182, DateTimeKind.Utc).AddTicks(9454) });

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
        }
    }
}
