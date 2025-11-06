using ETL.Worker.Application.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ETL.Worker.Worker
{

    public class ExtractionWorker : BackgroundService
    {
        private readonly ILogger<ExtractionWorker> _logger;
        private readonly ExtractionCoordinator _coordinator;

        public ExtractionWorker(ExtractionCoordinator coordinator, ILogger<ExtractionWorker> logger)
        {
            _coordinator = coordinator;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 ExtractionWorker iniciado a las {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Iniciando ciclo ETL...");
                    int total = await _coordinator.RunExtractionCycleAsync(stoppingToken);
                    _logger.LogInformation("Ciclo ETL completado correctamente. Registros procesados: {total}", total);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante el ciclo de extracción");
                }


                _logger.LogInformation("Esperando 5 minutos antes del próximo ciclo...");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }

            _logger.LogInformation("🔴 ExtractionWorker detenido a las {time}", DateTimeOffset.Now);
        }
    }
}
