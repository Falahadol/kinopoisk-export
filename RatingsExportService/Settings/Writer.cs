namespace RatingsExportService.Settings
{
    internal class Writer
    {
        public int TimeoutMs { get; set; } = 5000;
        public string Path { get; set; } = string.Empty;
    }
}
