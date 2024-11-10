using Microsoft.Extensions.DependencyInjection;
using RatingsExportService;
using Microsoft.Extensions.Hosting;
using RatingsExportService.Client;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
        services.AddHttpClient<IKinopoiskHttpClient, KinopoiskHttpClient>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();