using Dapper;
using ETL.Worker.Domain.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Infrastructure.Repositories
{
    public class StagingRepository
    {
        private readonly ILogger<StagingRepository> _logger;
        private readonly string _tempFolder;
        private readonly string _connectionString;

        public StagingRepository(IConfiguration config, ILogger<StagingRepository> logger)
        {
            _logger = logger;
            _tempFolder = config["Extraction:Staging:TempFolder"] ?? "Data/staging";
            _connectionString = config["Extraction:Database:ConnectionString"] ?? "Data Source=Database/etl_data.db;";
            Directory.CreateDirectory(_tempFolder);

            // Crear tabla si no existe
            using var conn = new SqliteConnection(_connectionString);
            conn.Execute(@"CREATE TABLE IF NOT EXISTS staging (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Source TEXT,
                UserName TEXT,
                Rating INTEGER,
                Comment TEXT,
                CreatedAt TEXT,
                InsertedAt TEXT DEFAULT (datetime('now'))
            );");
        }

        public async Task SaveToTempAsync(IEnumerable<RecordModel> records, string source, CancellationToken cancellationToken = default)
        {
            var file = Path.Combine(_tempFolder, $"{source}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            await File.WriteAllTextAsync(file, JsonSerializer.Serialize(records), cancellationToken);
            _logger.LogInformation("Saved {count} records to temp file {file}", records.Count(), file);
        }

        public async Task SaveToStagingTableAsync(IEnumerable<RecordModel> records, CancellationToken cancellationToken = default)
        {
            using var conn = new SqliteConnection(_connectionString);
            foreach (var r in records)
            {
                await conn.ExecuteAsync(
                    "INSERT INTO staging (Source, UserName, Rating, Comment, CreatedAt) VALUES (@Source, @UserName, @Rating, @Comment, @CreatedAt)",
                    new { r.Source, r.UserName, r.Rating, r.Comment, CreatedAt = r.CreatedAt.ToString("s") });
            }
            _logger.LogInformation("Inserted {count} records into SQLite staging table", records.Count());
        }
    }
}
