using Flurl;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using RatingsExportService.Settings;

namespace RatingsExportService.Clients
{
    internal class KinopoiskHttpClient: HttpClient, IKinopoiskHttpClient
    {
        private readonly HttpClient _client;
        private readonly IOptions<Client> _settings;

        public KinopoiskHttpClient(HttpClient client, IOptions<Client> settings)
        {
            _client = client;
            _settings = settings;
        }

        public async Task<HtmlDocument> GetPage(int page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GenerateUrl(page));
            request.Headers.Add("User-Agent", _settings.Value.UserAgent);
            request.Headers.Add("Cookie", _settings.Value.Cookie);

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStreamAsync();
            var document = new HtmlDocument();
            document.Load(html);

            return document;
        }

        private string? GetCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookie))
            {
                string cookie = string.Empty;
                foreach (var setCookieString in setCookie)
                {
                    var cookieTokens = setCookieString.Split(';');
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        cookie += "; ";
                    }
                    cookie += cookieTokens.FirstOrDefault();
                }

                if (!cookie.Contains("_yasc"))
                {
                    cookie += "; " + _settings.Value.Cookie;
                }
                return cookie;
            }
            return null;
        }

        private string GenerateUrl(int page)
        {
            if (page < 1)
                throw new ArgumentException(nameof(page));

            return Url.Combine(_settings.Value.Url,
                $"user/{_settings.Value.User}",
                $"votes/list/year_from/1921/year_to/{DateTime.UtcNow.Year}",
                $"perpage/200/page/{page}/#list");
        }

        public string? BaseUrl => _settings.Value.Url;
    }
}
