using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using YemenBooking.Application.DTOs;

namespace YemenBooking.Application.Interfaces.Services
{
    /// <summary>
    /// Service for managing city settings
    /// </summary>
    public interface ICitySettingsService
    {
        /// <summary>
        /// Get the list of cities
        /// </summary>
        Task<List<CityDto>> GetCitiesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Save or update the list of cities
        /// </summary>
        Task SaveCitiesAsync(List<CityDto> cities, CancellationToken cancellationToken = default);
    }
} 