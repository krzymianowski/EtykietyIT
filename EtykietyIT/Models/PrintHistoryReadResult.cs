namespace EtykietyIT.Models;

public sealed record PrintHistoryReadResult
{
    public IReadOnlyList<PrintHistoryEntry> Entries { get; init; } =
        Array.Empty<PrintHistoryEntry>();

    public int SkippedRecordCount { get; init; }
}
