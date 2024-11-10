using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RatingsExportService.Client;

namespace RatingsExportService
{
    internal class Worker: BackgroundService
    {
        public Worker(IKinopoiskHttpClient client, ILogger<Worker> logger)
        {
            _client = client;
            _logger = logger;
        }

        private readonly IKinopoiskHttpClient _client;
        private readonly ILogger<Worker> _logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var html = await _client.GetPage(1);
            var errorsCount = html.ParseErrors.Count();
            if (errorsCount > 0)
            {
                _logger.LogDebug("The HTML is NOT valid");
            }

            var fullCount = html.DocumentNode.SelectSingleNode("//table[@class='fontsize10']")?.SelectSingleNode("tr")?.SelectNodes("td").Last()?.InnerText;

            _logger.LogDebug("Count of movies {count}", fullCount);
        }
    }
}
