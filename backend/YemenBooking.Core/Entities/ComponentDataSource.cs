using System;

namespace YemenBooking.Core.Entities
{
    public class ComponentDataSource : BaseEntity
    {
        public Guid ComponentId { get; private set; }
        public string SourceType { get; private set; } // Static, API, Database, Cache
        public string DataEndpoint { get; private set; }
        public string HttpMethod { get; private set; }
        public string Headers { get; private set; } // JSON
        public string QueryParams { get; private set; } // JSON
        public string RequestBody { get; private set; } // JSON
        public string DataMapping { get; private set; } // JSON field mapping
        public string CacheKey { get; private set; }
        public int CacheDuration { get; private set; } // Minutes
        public string RefreshTrigger { get; private set; } // OnLoad, OnFocus, Manual, Timer
        public int RefreshInterval { get; private set; } // Seconds
        public string ErrorHandling { get; private set; } // JSON
        public string MockData { get; private set; } // JSON for development
        public bool UseMockInDev { get; private set; }
        
        public HomeScreenComponent Component { get; private set; }

        protected ComponentDataSource() { }

        public ComponentDataSource(
            Guid componentId,
            string sourceType,
            string dataEndpoint = null,
            string dataMapping = null)
        {
            Id = Guid.NewGuid();
            ComponentId = componentId;
            SourceType = sourceType;
            DataEndpoint = dataEndpoint;
            DataMapping = dataMapping;
            HttpMethod = "GET";
            RefreshTrigger = "OnLoad";
            CacheDuration = 5;
            UseMockInDev = true;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateEndpoint(
            string dataEndpoint,
            string httpMethod,
            string headers,
            string queryParams,
            string requestBody)
        {
            DataEndpoint = dataEndpoint;
            HttpMethod = httpMethod;
            Headers = headers;
            QueryParams = queryParams;
            RequestBody = requestBody;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCaching(
            string cacheKey,
            int cacheDuration,
            string refreshTrigger,
            int refreshInterval)
        {
            CacheKey = cacheKey;
            CacheDuration = cacheDuration;
            RefreshTrigger = refreshTrigger;
            RefreshInterval = refreshInterval;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMapping(string dataMapping)
        {
            DataMapping = dataMapping;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetMockData(string mockData, bool useMockInDev)
        {
            MockData = mockData;
            UseMockInDev = useMockInDev;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}