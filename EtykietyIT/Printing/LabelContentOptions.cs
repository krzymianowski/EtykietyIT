namespace EtykietyIT.Printing;

public sealed record LabelContentOptions(
    string CompanyName,
    string AssetIdPrefix,
    int AssetIdDigits,
    bool QrEnabled = false)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(CompanyName))
        {
            throw new InvalidOperationException("Nazwa firmy nie może być pusta.");
        }

        if (string.IsNullOrWhiteSpace(AssetIdPrefix))
        {
            throw new InvalidOperationException("Prefiks Asset ID nie może być pusty.");
        }

        if (AssetIdDigits < 1)
        {
            throw new InvalidOperationException("Liczba cyfr Asset ID musi być dodatnia.");
        }
    }
}
