namespace EtykietyIT.Models;

public sealed record AssetIdSettings
{
    public const int MinimumDigits = 1;
    public const int MaximumDigits = 12;

    public string Prefix { get; init; } = "IT-";

    public int Digits { get; init; } = 6;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Prefix))
        {
            throw new InvalidOperationException("Prefiks Asset ID nie może być pusty.");
        }

        if (Digits is < MinimumDigits or > MaximumDigits)
        {
            throw new InvalidOperationException(
                $"Liczba cyfr Asset ID musi mieścić się w zakresie " +
                $"{MinimumDigits}-{MaximumDigits}.");
        }
    }
}
