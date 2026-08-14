using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using EtykietyIT.Models;

namespace EtykietyIT.Services;

public sealed class PrintHistoryService
{
    private static readonly UTF8Encoding Utf8WithoutBom = new(false);

    private readonly string _historyFilePath;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly SemaphoreSlim _fileLock = new(1, 1);

    public PrintHistoryService(string historyFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(historyFilePath);
        _historyFilePath = Path.GetFullPath(historyFilePath);
        _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public void Append(PrintHistoryEntry entry)
    {
        byte[] line = SerializeLine(entry);

        _fileLock.Wait();
        try
        {
            EnsureHistoryDirectoryExists();
            using var stream = new FileStream(
                _historyFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 4096,
                FileOptions.WriteThrough);
            stream.Write(line);
            stream.Flush(flushToDisk: true);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task AppendAsync(
        PrintHistoryEntry entry,
        CancellationToken cancellationToken = default)
    {
        byte[] line = SerializeLine(entry);

        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            EnsureHistoryDirectoryExists();
            await using var stream = new FileStream(
                _historyFilePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.Read,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough);
            await stream.WriteAsync(line, CancellationToken.None);
            await stream.FlushAsync(CancellationToken.None);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public PrintHistoryReadResult ReadAll()
    {
        _fileLock.Wait();
        try
        {
            if (!File.Exists(_historyFilePath))
            {
                return new PrintHistoryReadResult();
            }

            using var stream = new FileStream(
                _historyFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);
            using var reader = new StreamReader(
                stream,
                Utf8WithoutBom,
                detectEncodingFromByteOrderMarks: true);

            var entries = new List<PrintHistoryEntry>();
            int skippedRecordCount = 0;
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                ReadLine(line, entries, ref skippedRecordCount);
            }

            return CreateReadResult(entries, skippedRecordCount);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<PrintHistoryReadResult> ReadAllAsync(
        CancellationToken cancellationToken = default)
    {
        await _fileLock.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_historyFilePath))
            {
                return new PrintHistoryReadResult();
            }

            await using var stream = new FileStream(
                _historyFilePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            using var reader = new StreamReader(
                stream,
                Utf8WithoutBom,
                detectEncodingFromByteOrderMarks: true);

            var entries = new List<PrintHistoryEntry>();
            int skippedRecordCount = 0;
            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                ReadLine(line, entries, ref skippedRecordCount);
            }

            return CreateReadResult(entries, skippedRecordCount);
        }
        finally
        {
            _fileLock.Release();
        }
    }

    private byte[] SerializeLine(PrintHistoryEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        entry.Validate();

        byte[] json = JsonSerializer.SerializeToUtf8Bytes(
            entry,
            _serializerOptions);
        var line = new byte[json.Length + 1];
        Buffer.BlockCopy(json, 0, line, 0, json.Length);
        line[^1] = (byte)'\n';
        return line;
    }

    private void ReadLine(
        string line,
        ICollection<PrintHistoryEntry> entries,
        ref int skippedRecordCount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new InvalidDataException("Pusta linia historii.");
            }

            PrintHistoryEntry entry = JsonSerializer.Deserialize<PrintHistoryEntry>(
                line,
                _serializerOptions) ?? throw new InvalidDataException(
                    "Linia historii nie zawiera wpisu.");
            entry.Validate();
            entries.Add(entry);
        }
        catch (Exception exception) when (
            exception is JsonException or InvalidDataException or
            InvalidOperationException or NotSupportedException)
        {
            skippedRecordCount++;
        }
    }

    private static PrintHistoryReadResult CreateReadResult(
        IReadOnlyList<PrintHistoryEntry> entries,
        int skippedRecordCount)
    {
        return new PrintHistoryReadResult
        {
            Entries = entries,
            SkippedRecordCount = skippedRecordCount
        };
    }

    private void EnsureHistoryDirectoryExists()
    {
        string? directoryPath = Path.GetDirectoryName(_historyFilePath);
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new InvalidOperationException(
                "Nie można ustalić katalogu historii wydruków.");
        }

        Directory.CreateDirectory(directoryPath);
    }
}
