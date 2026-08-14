namespace EtykietyIT.Models;

public sealed record OrganizationProfileReadResult
{
    public IReadOnlyList<OrganizationProfile> Profiles { get; init; } =
        Array.Empty<OrganizationProfile>();

    public int SkippedFileCount { get; init; }
}
