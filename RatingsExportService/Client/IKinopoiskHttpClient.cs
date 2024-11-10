using HtmlAgilityPack;

namespace RatingsExportService.Client
{
    internal interface IKinopoiskHttpClient
    {
        Task<HtmlDocument> GetPage(int page);
    }
}