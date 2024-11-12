using Flurl;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatingsExportService.Clients;
using RatingsExportService.Settings;
using RatingsExportService.Writers;

namespace RatingsExportService
{
    internal class MainService: BackgroundService
    {
        public MainService(IKinopoiskHttpClient client, IFileWriter writer, IOptions<Worker> settings, ILogger<MainService> logger)
        {
            _client = client;
            _writer = writer;
            _settings = settings;
            _logger = logger;
        }

        private readonly IKinopoiskHttpClient _client;
        private readonly ILogger<MainService> _logger;
        private readonly IFileWriter _writer;
        private readonly IOptions<Worker> _settings;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var html = await _client.GetPage(_settings.Value.StartPage);
            Write(html);

            var fullCount = html.DocumentNode.SelectSingleNode("//table[@class='fontsize10']")?.SelectSingleNode("tr")?.SelectNodes("td").Last()?.InnerText ?? "0";

            _logger.LogDebug("Count of movies {count}", fullCount);

            var count = int.Parse(fullCount) / 200;
            if (count % 200 > 0)
            {
                count++;
            }
            var rand = new Random();
            for (var i = _settings.Value.StartPage + 1; i <= count; i++)
            {
                await Task.Delay(rand.Next(20000, 40000));
                html = await _client.GetPage(i);
                Write(html);
            }

            await _writer.Finish();
        }

        private void Write(HtmlDocument html)
        {
            foreach (var itemDiv in html.DocumentNode.SelectNodes("//div[@class='item even' or @class='item']"))
            {
                var date = itemDiv.SelectSingleNode("div[@class='date']")?.InnerText;
                var vote = itemDiv.SelectSingleNode("div[@class='vote']")?.InnerText;
                var infoDiv = itemDiv.SelectSingleNode("div[@class='info']");
                var nameEng = infoDiv.SelectSingleNode("div[@class='nameEng']")?.InnerText?.Replace("&nbsp;", " ");
                var nameRusDiv = infoDiv.SelectSingleNode("div[@class='nameRus']")?.FirstChild;
                var nameRus = nameRusDiv?.InnerText?.Replace("&nbsp;", " ");
                var url = nameRusDiv?.GetAttributeValue("href", null);

                _writer.Write(new MovieRecord()
                {
                    Date = date,
                    Vote = vote,
                    NameEng = nameEng.Fix(),
                    NameRus = nameRus.Fix(),
                    Url = Url.Combine(_client.BaseUrl, url)
                });
            }
        }
    }
}
