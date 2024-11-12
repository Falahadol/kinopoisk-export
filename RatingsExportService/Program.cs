using Microsoft.Extensions.DependencyInjection;
using RatingsExportService;
using Microsoft.Extensions.Hosting;
using RatingsExportService.Settings;
using RatingsExportService.Clients;
using RatingsExportService.Writers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<Client>(context.Configuration.GetSection(nameof(Client)));
        services.Configure<Writer>(context.Configuration.GetSection(nameof(Writer)));
        services.Configure<Worker>(context.Configuration.GetSection(nameof(Worker)));
        services.AddHttpClient<IKinopoiskHttpClient, KinopoiskHttpClient>();
        services.AddSingleton<IFileWriter, FileWriter>();
        services.AddHostedService<MainService>();
    })
    .Build();

await host.RunAsync();