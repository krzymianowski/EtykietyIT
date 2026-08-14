namespace EtykietyIT.Services;

public static class AssetIdFormatter
{
    private const string Prefix = "IT-";
    private const int Digits = 6;

    public static string Format(int number)
    {
        return Prefix + number.ToString($"D{Digits}");
    }
}
