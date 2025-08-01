using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;
using YemenBooking.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using YemenBooking.Infrastructure.Data.Context;
using YemenBooking.Infrastructure.Repositories;

namespace Infrastructure.Repositories
{
    public class HomeScreenRepository : IHomeScreenRepository
    {
        private readonly YemenBookingDbContext _context;

        public HomeScreenRepository(YemenBookingDbContext context)
        {
            _context = context;
        }

        public async Task<HomeScreenTemplate> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetTemplateWithSectionsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Include(t => t.Sections.Where(s => !s.IsDeleted))
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetTemplateWithFullHierarchyAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Include(t => t.Sections.Where(s => !s.IsDeleted))
                    .ThenInclude(s => s.Components.Where(c => !c.IsDeleted))
                        .ThenInclude(c => c.Properties)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.Styles)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.Actions)
                .Include(t => t.Sections)
                    .ThenInclude(s => s.Components)
                        .ThenInclude(c => c.DataSource)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<List<HomeScreenTemplate>> GetTemplatesAsync(
            string platform,
            string targetAudience,
            bool? isActive,
            bool includeDeleted,
            CancellationToken cancellationToken)
        {
            var query = _context.HomeScreenTemplates.AsQueryable();

            if (!includeDeleted)
                query = query.Where(t => !t.IsDeleted);

            if (!string.IsNullOrEmpty(platform))
                query = query.Where(t => t.Platform == platform || t.Platform == "All");

            if (!string.IsNullOrEmpty(targetAudience))
                query = query.Where(t => t.TargetAudience == targetAudience || t.TargetAudience == "All");

            if (isActive.HasValue)
                query = query.Where(t => t.IsActive == isActive.Value);

            return await query
                .OrderByDescending(t => t.IsDefault)
                .ThenByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<HomeScreenTemplate>> GetActiveTemplatesAsync(
            string platform,
            string targetAudience,
            CancellationToken cancellationToken)
        {
            return await GetTemplatesAsync(platform, targetAudience, true, false, cancellationToken);
        }

        public async Task<HomeScreenTemplate> GetActiveTemplateAsync(
            string platform,
            string targetAudience,
            CancellationToken cancellationToken)
        {
            var templates = await GetActiveTemplatesAsync(platform, targetAudience, cancellationToken);
            return templates.FirstOrDefault();
        }

        public async Task<HomeScreenTemplate> GetDefaultTemplateAsync(string platform, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenTemplates
                .Where(t => !t.IsDeleted && t.IsDefault && (t.Platform == platform || t.Platform == "All"))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<HomeScreenSection> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<HomeScreenSection> GetSectionWithComponentsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .Include(s => s.Components.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
        }

        public async Task<List<HomeScreenSection>> GetTemplateSectionsAsync(Guid templateId, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenSections
                .Where(s => s.TemplateId == templateId && !s.IsDeleted)
                .OrderBy(s => s.Order)
                .ToListAsync(cancellationToken);
        }

        public async Task<HomeScreenComponent> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.HomeScreenComponents
                .Include(c => c.Properties)
                .Include(c => c.Styles)
                .Include(c => c.Actions)
                .Include(c => c.DataSource)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken) ;
        }

        public async Task<UserHomeScreen> GetUserHomeScreenAsync(Guid userId, string platform, CancellationToken cancellationToken)
        {
            return await _context.UserHomeScreens
                .Include(u => u.Template)
                .FirstOrDefaultAsync(u => u.UserId == userId && 
                    (u.Template.Platform == platform || u.Template.Platform == "All") &&
                    u.Template.IsActive &&
                    !u.Template.IsDeleted, 
                    cancellationToken);
        }

        public async Task AddTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken)
        {
            await _context.HomeScreenTemplates.AddAsync(template, cancellationToken);
        }

        public async Task AddSectionAsync(HomeScreenSection section, CancellationToken cancellationToken)
        {
            await _context.HomeScreenSections.AddAsync(section, cancellationToken);
        }

        public async Task AddComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken)
        {
            await _context.HomeScreenComponents.AddAsync(component, cancellationToken);
        }

        public async Task UpdateTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken)
        {
            _context.HomeScreenTemplates.Update(template);
        }

        public async Task UpdateSectionAsync(HomeScreenSection section, CancellationToken cancellationToken)
        {
            _context.HomeScreenSections.Update(section);
        }

        public async Task UpdateSectionsAsync(List<HomeScreenSection> sections, CancellationToken cancellationToken)
        {
            _context.HomeScreenSections.UpdateRange(sections);
        }

        public async Task UpdateComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken)
        {
            _context.HomeScreenComponents.Update(component);
        }

        public async Task UpdateComponentsAsync(List<HomeScreenComponent> components, CancellationToken cancellationToken)
        {
            _context.HomeScreenComponents.UpdateRange(components);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}