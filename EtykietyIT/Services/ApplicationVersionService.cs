using System.Reflection;

namespace EtykietyIT.Services;

public sealed class ApplicationVersionService
{
    public ApplicationVersionService()
    {
        Assembly applicationAssembly = typeof(ApplicationVersionService).Assembly;
        DiagnosticVersion = applicationAssembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ??
            applicationAssembly.GetName().Version?.ToString() ??
            throw new InvalidOperationException(
                "Nie można odczytać wersji aplikacji z metadanych assembly.");

        int metadataSeparatorIndex = DiagnosticVersion.IndexOf(
            '+',
            StringComparison.Ordinal);
        UserVersion = (metadataSeparatorIndex >= 0
                ? DiagnosticVersion[..metadataSeparatorIndex]
                : DiagnosticVersion)
            .Trim();

        if (string.IsNullOrWhiteSpace(UserVersion))
        {
            throw new InvalidOperationException(
                "Metadane assembly nie zawierają użytkowej wersji aplikacji.");
        }
    }

    public string UserVersion { get; }

    public string DiagnosticVersion { get; }
}
