
namespace RatingsExportService.Writers
{
    internal interface IFileWriter
    {
        Task Finish();
        void Write(MovieRecord record);
    }
}