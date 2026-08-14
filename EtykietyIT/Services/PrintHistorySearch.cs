using System.Globalization;
using EtykietyIT.Models;

namespace EtykietyIT.Services;

public static class PrintHistorySearch
{
    public static bool Matches(PrintHistoryEntry entry, string? searchText)
    {
        ArgumentNullException.ThrowIfNull(entry);

        string query = searchText?.Trim() ?? string.Empty;
        if (query.Length == 0)
        {
            return true;
        }

        PrintHistorySnapshot snapshot = entry.Snapshot;
        if (Contains(snapshot.PrinterName, query) ||
            Contains(snapshot.ProfileName, query) ||
            Contains(snapshot.ProfileId, query) ||
            Contains(snapshot.CompanyName, query) ||
            Contains(snapshot.FirstAssetId, query) ||
            Contains(snapshot.LastAssetId, query))
        {
            return true;
        }

        return TryParseAssetNumber(snapshot, query, out int assetNumber) &&
            assetNumber >= snapshot.StartNumber &&
            assetNumber <= snapshot.EndNumber;
    }

    private static bool TryParseAssetNumber(
        PrintHistorySnapshot snapshot,
        string query,
        out int assetNumber)
    {
        assetNumber = default;
        if (!query.StartsWith(snapshot.Prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        ReadOnlySpan<char> numberText = query.AsSpan(snapshot.Prefix.Length);
        return numberText.Length > 0 && int.TryParse(
            numberText,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out assetNumber);
    }

    private static bool Contains(string value, string query)
    {
        return value.Contains(query, StringComparison.CurrentCultureIgnoreCase);
    }
}
