using Flurl;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;

namespace RatingsExportService.Client
{
    internal class KinopoiskHttpClient: HttpClient, IKinopoiskHttpClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<Settings> _settings;

        public KinopoiskHttpClient(HttpClient client, IOptions<Settings> settings)
        {
            _client = client;
            _settings = settings;
        }

        public async Task<HtmlDocument> GetPage(int page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, generateUrl(page));
            request.Headers.Add("User-Agent", _settings.Value.UserAgent);
            request.Headers.Add("Cookie", _settings.Value.Cookie);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStreamAsync();

            var document = new HtmlDocument();
            document.Load(html);

            return document;
        }

        private string generateUrl(int page)
        {
            if (page < 1)
                throw new ArgumentException(nameof(page));

            return Url.Combine(_settings.Value.Url,
                $"user/{_settings.Value.User}",
                $"votes/list/year_from/1921/year_to/{DateTime.UtcNow.Year}",
                $"perpage/200/page/{page}/#list");
        }
    }
}
