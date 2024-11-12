using CsvHelper.Configuration.Attributes;

namespace RatingsExportService
{
    internal class MovieRecord
    {
        [Index(1)]
        public string? NameRus { get; set; }
        [Index(2)]
        public string? NameEng { get; set; }
        [Index(3)]
        public string? Vote { get; set; }
        [Index(4)]
        public string? Date { get; set; }
        [Index(5)]
        public string? Url { get; set; }
    }
}
