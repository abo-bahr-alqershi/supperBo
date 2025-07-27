using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using YemenBooking.Application.DTOs;
using YemenBooking.Application.Interfaces.Services;

namespace YemenBooking.Infrastructure.Services
{
    /// <summary>
    /// Implementation of currency settings service using a protected JSON file
    /// </summary>
    public class CurrencySettingsService : ICurrencySettingsService
    {
        private readonly string _currenciesFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public CurrencySettingsService(IHostEnvironment env)
        {
            var settingsDir = Path.Combine(env.ContentRootPath, "Settings");
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }
            _currenciesFilePath = Path.Combine(settingsDir, "currencies.json");
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<CurrencyDto>> GetCurrenciesAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_currenciesFilePath))
            {
                var defaultList = new List<CurrencyDto>();
                var defaultContent = JsonSerializer.Serialize(defaultList, _jsonOptions);
                await File.WriteAllTextAsync(_currenciesFilePath, defaultContent, cancellationToken);
                return defaultList;
            }

            var content = await File.ReadAllTextAsync(_currenciesFilePath, cancellationToken);
            return JsonSerializer.Deserialize<List<CurrencyDto>>(content, _jsonOptions) ?? new List<CurrencyDto>();
        }

        public async Task SaveCurrenciesAsync(List<CurrencyDto> currencies, CancellationToken cancellationToken = default)
        {
            if (currencies == null) throw new ArgumentNullException(nameof(currencies));

            if (currencies.Count(c => c.IsDefault) != 1)
                throw new InvalidOperationException("Exactly one default currency must be specified.");

            List<CurrencyDto> existing = new List<CurrencyDto>();
            if (File.Exists(_currenciesFilePath))
            {
                existing = await GetCurrenciesAsync(cancellationToken);
                var existingDefault = existing.FirstOrDefault(c => c.IsDefault);
                var incomingDefault = currencies.First(c => c.IsDefault);
                if (existingDefault != null && existingDefault.Code != incomingDefault.Code)
                    throw new InvalidOperationException("Default currency cannot be changed once initialized.");
            }

            var updatedList = new List<CurrencyDto>();
            foreach (var currency in currencies)
            {
                var existingCurrency = existing.FirstOrDefault(c => c.Code == currency.Code);
                DateTime? lastUpdated = existingCurrency?.LastUpdated;
                if (!currency.IsDefault)
                {
                    if (existingCurrency == null || existingCurrency.ExchangeRate != currency.ExchangeRate)
                    {
                        lastUpdated = DateTime.UtcNow;
                    }
                }
                else
                {
                    // default currency, no exchange rate
                    currency.ExchangeRate = null;
                }

                updatedList.Add(new CurrencyDto
                {
                    Code = currency.Code,
                    ArabicCode = currency.ArabicCode,
                    Name = currency.Name,
                    ArabicName = currency.ArabicName,
                    IsDefault = currency.IsDefault,
                    ExchangeRate = currency.ExchangeRate,
                    LastUpdated = lastUpdated
                });
            }

            var content = JsonSerializer.Serialize(updatedList, _jsonOptions);
            await File.WriteAllTextAsync(_currenciesFilePath, content, cancellationToken);
        }
    }
} 