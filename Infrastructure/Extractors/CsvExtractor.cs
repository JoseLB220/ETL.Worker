using CsvHelper;
using CsvHelper.Configuration;
using ETL.Worker.Application.Interfaces;
using ETL.Worker.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Infrastructure.Extractors
{
    public class CsvExtractor : IExtractor
    {
        private readonly ILogger<CsvExtractor> _logger;
        private readonly string _path;

        public CsvExtractor(IConfiguration config, ILogger<CsvExtractor> logger)
        {
            _logger = logger;
            _path = config["Extraction:Csv:Path"] ?? "Data/input/encuestas.csv";
        }

        public async Task<IEnumerable<RecordModel>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            var records = new List<RecordModel>();
            if (!File.Exists(_path))
            {
                _logger.LogWarning("CSV file not found: {path}", _path);
                return records;
            }

            using var reader = new StreamReader(_path);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            // Map CSV columns dynamically or with class map
            await foreach (var row in csv.GetRecordsAsync<dynamic>().WithCancellation(cancellationToken))
            {
                try
                {
                    var dict = (IDictionary<string, object>)row;
                    var model = new RecordModel
                    {
                        Source = "csv",
                        UserName = dict.ContainsKey("UserName") ? dict["UserName"]?.ToString() ?? "" : "",
                        Comment = dict.ContainsKey("Comment") ? dict["Comment"]?.ToString() ?? "" : "",
                        Rating = dict.ContainsKey("Rating") && int.TryParse(dict["Rating"]?.ToString(), out var r) ? r : 0,
                        CreatedAt = dict.ContainsKey("CreatedAt") && DateTime.TryParse(dict["CreatedAt"]?.ToString(), out var d) ? d : DateTime.UtcNow
                    };
                    records.Add(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing CSV row");
                }
            }

            _logger.LogInformation("CSV Extraction produced {count} records", records.Count);
            return records;
        }
    }
}
