
using ETL.Worker.Application.Interfaces;
using ETL.Worker.Domain.Models;
using ETL.Worker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Application.Services
{
    public class ExtractionCoordinator
    {
        private readonly IEnumerable<IExtractor> _extractors;
        private readonly StagingRepository _staging;
        private readonly ILogger<ExtractionCoordinator> _logger;

        public ExtractionCoordinator(IEnumerable<IExtractor> extractors, StagingRepository staging, ILogger<ExtractionCoordinator> logger)
        {
            _extractors = extractors;
            _staging = staging;
            _logger = logger;
        }

        
        public async Task<int> RunExtractionCycleAsync(CancellationToken ct = default)
        {
            var tasks = _extractors.Select(e => e.ExtractAsync(ct)).ToList();
            var results = await Task.WhenAll(tasks);
            int total = 0;
            foreach (var r in results)
            {
                var list = r.ToList();
                if (!list.Any()) continue;
                var source = list.First().Source ?? "unknown";
                await _staging.SaveToTempAsync(list, source, ct);
                await _staging.SaveToStagingTableAsync(list, ct);
                total += list.Count;
            }
            _logger.LogInformation("ExtractionCoordinator: procesados {total} registros", total);
            return total;
        }
    }
}
