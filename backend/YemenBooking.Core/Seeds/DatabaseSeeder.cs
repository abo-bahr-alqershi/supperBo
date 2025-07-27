using Microsoft.EntityFrameworkCore;
using YemenBooking.Core.Entities;
using System;
using YemenBooking.Core.Seeds;
using System.Linq;

namespace YemenBooking.Core.Seeds
{
    /// <summary>
    /// فئة لتجميع وتشغيل جميع مولدات البيانات الأولية
    /// </summary>
    public static class DatabaseSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var userSeeder = new UserSeeder();
            modelBuilder.Entity<User>().HasData(userSeeder.SeedData());

            // Static seeding for bookings removed to avoid owned type seed issues
            // Seed user roles
            modelBuilder.Entity<UserRole>().HasData(
                new
                {
                    Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                    UserId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                    RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    AssignedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                },
                new
                {
                    Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                    UserId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                    RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    AssignedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                }
            );
        }
    }
} 