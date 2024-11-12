using HtmlAgilityPack;

namespace RatingsExportService.Clients
{
    internal interface IKinopoiskHttpClient
    {
        string? BaseUrl { get; }

        Task<HtmlDocument> GetPage(int page);
    }
}