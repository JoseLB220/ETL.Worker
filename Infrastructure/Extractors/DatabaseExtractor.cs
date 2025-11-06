using Dapper;
using ETL.Worker.Application.Interfaces;
using ETL.Worker.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Infrastructure.Extractors
{
    public class DatabaseExtractor : IExtractor
    {
        private readonly ILogger<DatabaseExtractor> _logger;
        private readonly string _connectionString;
        private readonly string _query;

        public DatabaseExtractor(IConfiguration configuration, ILogger<DatabaseExtractor> logger)
        {
            _logger = logger;
            _connectionString = configuration["Extraction:Database:ConnectionString"]!;
            _query = configuration["Extraction:Database:Query"] ?? "SELECT Id, UserName, Rating, Comment, CreatedAt FROM staging";
        }

        public async Task<IEnumerable<RecordModel>> ExtractAsync(CancellationToken cancellationToken = default)
        {
            using var conn = new SqliteConnection(_connectionString);
            var rows = await conn.QueryAsync<dynamic>(_query);
            var list = new List<RecordModel>();

            foreach (var r in rows)
            {
                int id = Convert.ToInt32(r.Id);
                int rating = 0;
                try { rating = Convert.ToInt32(r.Rating); } catch { }

                DateTime parsedDate;
                if (!DateTime.TryParse(r.CreatedAt?.ToString(), out parsedDate))
                    parsedDate = DateTime.UtcNow;

                list.Add(new RecordModel
                {
                    Id = id,
                    Source = "sqlite",
                    UserName = r.UserName?.ToString() ?? "",
                    Rating = rating,
                    Comment = r.Comment?.ToString() ?? "",
                    CreatedAt = parsedDate
                });
            }

            _logger.LogInformation("SQLite Extraction returned {count} records", list.Count);
            return list;
        }
    }
}
