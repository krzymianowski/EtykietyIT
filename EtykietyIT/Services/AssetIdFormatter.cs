namespace EtykietyIT.Services;

public static class AssetIdFormatter
{
    public static string Format(int number, string prefix, int digits)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException(
                "Prefiks Asset ID nie może być pusty.",
                nameof(prefix));
        }

        if (digits < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(digits),
                digits,
                "Liczba cyfr Asset ID musi być dodatnia.");
        }

        return prefix + number.ToString($"D{digits}");
    }
}
