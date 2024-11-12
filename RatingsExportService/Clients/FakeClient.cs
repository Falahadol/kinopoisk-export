using HtmlAgilityPack;

namespace RatingsExportService.Clients
{
    internal class FakeClient: HttpClient, IKinopoiskHttpClient
    {
        public FakeClient(HttpClient client)
        { }

        public string? BaseUrl => "https://www.kinopoisk.ru/";

        public async Task<HtmlDocument> GetPage(int page)
        {
            var html = await File.ReadAllTextAsync(@"Clients\freeformatter-out.html");

            var document = new HtmlDocument();
            document.LoadHtml(html);

            return document;
        }
    }
}
