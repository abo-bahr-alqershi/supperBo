// Infrastructure/Repositories/IHomeScreenRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Core.Entities;

namespace YemenBooking.Core.Interfaces.Repositories
{
    public interface IHomeScreenRepository 
    {
        // Template operations
        Task<HomeScreenTemplate> GetTemplateByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetTemplateWithSectionsAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetTemplateWithFullHierarchyAsync(Guid id, CancellationToken cancellationToken);
        Task<List<HomeScreenTemplate>> GetTemplatesAsync(string platform, string targetAudience, bool? isActive, bool includeDeleted, CancellationToken cancellationToken);
        Task<List<HomeScreenTemplate>> GetActiveTemplatesAsync(string platform, string targetAudience, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetActiveTemplateAsync(string platform, string targetAudience, CancellationToken cancellationToken);
        Task<HomeScreenTemplate> GetDefaultTemplateAsync(string platform, CancellationToken cancellationToken);
        Task AddTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken);
        Task UpdateTemplateAsync(HomeScreenTemplate template, CancellationToken cancellationToken);

        // Section operations
        Task<HomeScreenSection> GetSectionByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<HomeScreenSection> GetSectionWithComponentsAsync(Guid id, CancellationToken cancellationToken);
        Task<List<HomeScreenSection>> GetTemplateSectionsAsync(Guid templateId, CancellationToken cancellationToken);
        Task AddSectionAsync(HomeScreenSection section, CancellationToken cancellationToken);
        Task UpdateSectionAsync(HomeScreenSection section, CancellationToken cancellationToken);
        Task UpdateSectionsAsync(List<HomeScreenSection> sections, CancellationToken cancellationToken);

        // Component operations
        Task<HomeScreenComponent> GetComponentByIdAsync(Guid id, CancellationToken cancellationToken);
        Task AddComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken);
        Task UpdateComponentAsync(HomeScreenComponent component, CancellationToken cancellationToken);
        Task UpdateComponentsAsync(List<HomeScreenComponent> components, CancellationToken cancellationToken);

        // User customization operations
        Task<UserHomeScreen> GetUserHomeScreenAsync(Guid userId, string platform, CancellationToken cancellationToken);

        // Save changes
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}