using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RatingsExportService.Settings;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace RatingsExportService.Writers
{
    internal class FileWriter: IFileWriter, IAsyncDisposable
    {
        private readonly ConcurrentQueue<MovieRecord> _objectToWrite = new();
        private readonly CancellationTokenSource _source = new();
        private readonly Task _task;
        private readonly ILogger<FileWriter> _logger;
        private readonly IOptions<Writer> _settings;

        public FileWriter(IOptions<Writer> settings, ILogger<FileWriter> logger)
        {
            if (string.IsNullOrEmpty(settings.Value.Path))
                throw new ArgumentNullException(nameof(settings.Value.Path));

            if (string.IsNullOrEmpty(Path.GetFileName(settings.Value.Path)))
                throw new ArgumentException(nameof(settings.Value.Path));

            _settings = settings;
            _logger = logger;
            _task = WriteTask();
        }

        public void Write(MovieRecord record)
        {
            if (_task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(_task.Exception.InnerException!).Throw();
            }
            _objectToWrite.Enqueue(record);
        }

        public async Task Finish()
        {
            _source.Cancel();
            await _task;
        }

        private async Task WriteTask()
        {
            _logger.LogInformation("Started writing on {path}", _settings.Value.Path);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            if (!File.Exists(_settings.Value.Path))
            {
                await WriteHeaderToFile(config);
            }
            config.HasHeaderRecord = false;

            while (!_source.Token.IsCancellationRequested)
            {
                await WriteToFile(config);
                await Task.Delay(_settings.Value.TimeoutMs);
            }
            if (!_objectToWrite.IsEmpty)
            {
                await WriteToFile(config);
            }
        }

        private async Task WriteHeaderToFile(CsvConfiguration config)
        {
            using var stream = File.Open(_settings.Value.Path, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);
            csv.WriteHeader<MovieRecord>();
            await csv.NextRecordAsync();
            await csv.FlushAsync();
        }

        private async Task WriteToFile(CsvConfiguration config)
        {
            var count = 0;
            using var stream = File.Open(_settings.Value.Path, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);
            while (_objectToWrite.TryDequeue(out var record))
            {
                csv.WriteRecord(record);
                await csv.NextRecordAsync();
                count++;
            }
            await csv.FlushAsync();
            _logger.LogInformation("Added {count} records", count);
        }

        public async ValueTask DisposeAsync()
        {
            await Finish();
        }
    }
}
