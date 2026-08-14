using EtykietyIT.Models;
using EtykietyIT.Persistence;
using EtykietyIT.Printing;

namespace EtykietyIT.Services;

public sealed class PrinterCalibrationService
{
    private readonly JsonFileStore _jsonFileStore;
    private readonly string _calibrationsFilePath;

    public PrinterCalibrationService(
        JsonFileStore jsonFileStore,
        string calibrationsFilePath)
    {
        _jsonFileStore = jsonFileStore ??
            throw new ArgumentNullException(nameof(jsonFileStore));
        ArgumentException.ThrowIfNullOrWhiteSpace(calibrationsFilePath);
        _calibrationsFilePath = Path.GetFullPath(calibrationsFilePath);
    }

    public async Task<PrinterCalibration> GetCalibrationAsync(
        string printerName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(printerName);

        PrinterCalibrationDocument document = await LoadDocumentAsync(
            cancellationToken);
        PrinterCalibrationEntry? entry = document.Printers.FirstOrDefault(
            item => string.Equals(
                item.PrinterName,
                printerName,
                StringComparison.OrdinalIgnoreCase));

        return entry is null
            ? new PrinterCalibration()
            : new PrinterCalibration(entry.OffsetXmm, entry.OffsetYmm);
    }

    public async Task SaveCalibrationAsync(
        string printerName,
        PrinterCalibration calibration,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(printerName);
        ArgumentNullException.ThrowIfNull(calibration);

        var updatedEntry = new PrinterCalibrationEntry
        {
            PrinterName = printerName,
            OffsetXmm = calibration.OffsetXmm,
            OffsetYmm = calibration.OffsetYmm
        };
        updatedEntry.Validate();

        PrinterCalibrationDocument document = await LoadDocumentAsync(
            cancellationToken);
        var entries = document.Printers.ToList();
        int existingIndex = entries.FindIndex(item => string.Equals(
            item.PrinterName,
            printerName,
            StringComparison.OrdinalIgnoreCase));

        if (existingIndex >= 0)
        {
            entries[existingIndex] = updatedEntry;
        }
        else
        {
            entries.Add(updatedEntry);
        }

        PrinterCalibrationDocument updatedDocument = document with
        {
            Printers = entries
        };

        await _jsonFileStore.SaveAsync(
            _calibrationsFilePath,
            updatedDocument,
            value => value.Validate(),
            cancellationToken);
    }

    private async Task<PrinterCalibrationDocument> LoadDocumentAsync(
        CancellationToken cancellationToken)
    {
        if (!File.Exists(_calibrationsFilePath))
        {
            return new PrinterCalibrationDocument();
        }

        PrinterCalibrationDocument document =
            await _jsonFileStore.LoadAsync<PrinterCalibrationDocument>(
                _calibrationsFilePath,
                cancellationToken) ?? throw new InvalidDataException(
                    "Plik printer-calibrations.json nie zawiera kalibracji.");

        document.Validate();
        return document;
    }
}
