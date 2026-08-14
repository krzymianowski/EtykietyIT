using System.Text.Json;
using System.Text.Json.Serialization;

namespace EtykietyIT.Persistence;

public sealed class JsonFileStore
{
    private readonly JsonSerializerOptions _serializerOptions;

    public JsonFileStore()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        _serializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }

    public async Task<T?> LoadAsync<T>(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
        {
            return default;
        }

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        return await JsonSerializer.DeserializeAsync<T>(
            stream,
            _serializerOptions,
            cancellationToken);
    }

    public async Task SaveAsync<T>(
        string filePath,
        T value,
        Action<T>? validate = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(value);

        string targetPath = Path.GetFullPath(filePath);
        string? directoryPath = Path.GetDirectoryName(targetPath);

        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new InvalidOperationException(
                "Nie można ustalić katalogu docelowego pliku JSON.");
        }

        Directory.CreateDirectory(directoryPath);

        string temporaryPath = Path.Combine(
            directoryPath,
            $".{Path.GetFileName(targetPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await using (var writeStream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(
                    writeStream,
                    value,
                    _serializerOptions,
                    cancellationToken);
                await writeStream.FlushAsync(cancellationToken);
            }

            T validatedValue;
            await using (var validationStream = new FileStream(
                temporaryPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                validatedValue = await JsonSerializer.DeserializeAsync<T>(
                    validationStream,
                    _serializerOptions,
                    cancellationToken) ?? throw new InvalidDataException(
                        "Walidacja tymczasowego pliku JSON zwróciła pusty dokument.");
            }

            validate?.Invoke(validatedValue);

            if (File.Exists(targetPath))
            {
                File.Replace(temporaryPath, targetPath, null, true);
            }
            else
            {
                File.Move(temporaryPath, targetPath);
            }
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
