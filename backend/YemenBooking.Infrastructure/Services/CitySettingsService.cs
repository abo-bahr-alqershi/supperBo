using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// Implementation of city settings service using a protected JSON file
    /// </summary>
    public class CitySettingsService : ICitySettingsService
    {
        private readonly string _citiesFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public CitySettingsService(IHostEnvironment env)
        {
            var settingsDir = Path.Combine(env.ContentRootPath, "Settings");
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }
            _citiesFilePath = Path.Combine(settingsDir, "cities.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<CityDto>> GetCitiesAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_citiesFilePath))
            {
                var defaultList = new List<CityDto>();
                var defaultContent = JsonSerializer.Serialize(defaultList, _jsonOptions);
                await File.WriteAllTextAsync(_citiesFilePath, defaultContent, cancellationToken);
                return defaultList;
            }

            var content = await File.ReadAllTextAsync(_citiesFilePath, cancellationToken);
            return JsonSerializer.Deserialize<List<CityDto>>(content, _jsonOptions) ?? new List<CityDto>();
        }

        public async Task SaveCitiesAsync(List<CityDto> cities, CancellationToken cancellationToken = default)
        {
            if (cities == null) throw new ArgumentNullException(nameof(cities));

            // Ensure city names are unique
            var duplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var city in cities)
            {
                if (!duplicates.Add(city.Name))
                    throw new InvalidOperationException($"Duplicate city name: {city.Name}");
            }

            var content = JsonSerializer.Serialize(cities, _jsonOptions);
            await File.WriteAllTextAsync(_citiesFilePath, content, cancellationToken);
        }
    }
} 