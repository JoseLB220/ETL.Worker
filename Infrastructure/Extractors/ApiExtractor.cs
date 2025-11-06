using ETL.Worker.Application.Interfaces;
using ETL.Worker.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Infrastructure.Extractors
{
    public class ApiExtractor : IExtractor
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiExtractor> _logger;
        private readonly string _endpoint;

        public ApiExtractor(HttpClient client, IConfiguration config, ILogger<ApiExtractor> logger)
        {
            _client = client;
            _logger = logger;
            var baseUrl = config["Extraction:Api:BaseUrl"] ?? "";
            _endpoint = config["Extraction:Api:Endpoint"] ?? "/comments";
            _client.BaseAddress = new Uri(baseUrl);
            // add API Key header if present
            var apiKey = config["Extraction:Api:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
                _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public async Task<IEnumerable<RecordModel>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<RecordModel>();
            try
            {
                var response = await _client.GetAsync(_endpoint, cancellationToken);
                response.EnsureSuccessStatusCode();
                var items = await response.Content.ReadFromJsonAsync<List<ApiCommentDto>>(cancellationToken: cancellationToken);
                if (items != null)
                {
                    result.AddRange(items.Select(i => new RecordModel
                    {
                        Source = "api",
                        UserName = i.User ?? "",
                        Comment = i.Text ?? "",
                        Rating = i.Rating,
                        CreatedAt = i.Timestamp
                    }));
                }
                _logger.LogInformation("API Extraction returned {count} records", result.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting from API");
            }
            return result;
        }

        private record ApiCommentDto(string User, string Text, int Rating, DateTime Timestamp);
    }
}
