using ETL.Worker.Application.Interfaces;
using ETL.Worker.Application.Services;
using ETL.Worker.Infrastructure.Extractors;
using ETL.Worker.Infrastructure.Repositories;
using ETL.Worker.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;


        services.AddTransient<IExtractor, CsvExtractor>();
        services.AddTransient<IExtractor, DatabaseExtractor>();
        services.AddHttpClient(); 


        services.AddTransient<IExtractor>(sp =>
        {
            var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var client = clientFactory.CreateClient("api-client");
            var cfg = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILogger<ApiExtractor>>();
            return new ApiExtractor(client, cfg, logger);
        });


        services.AddSingleton<StagingRepository>();


        services.AddSingleton<ExtractionCoordinator>();


        services.AddHostedService<ExtractionWorker>();


        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        });
    })
    .Build();

await host.RunAsync();
