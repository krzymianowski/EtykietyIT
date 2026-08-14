using EtykietyIT.Models;

namespace EtykietyIT.Export;

public interface IHistoryExporter
{
    Task ExportAsync(
        IEnumerable<PrintHistoryEntry> entries,
        string filePath,
        CancellationToken cancellationToken = default);
}
