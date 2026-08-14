using EtykietyIT.Models;

namespace EtykietyIT.Persistence;

public sealed class ApplicationModeDetector
{
    public const string PortableArgument = "--portable";
    public const string PortableMarkerFileName = "portable.mode";

    public ApplicationMode Detect(
        IEnumerable<string> arguments,
        string executableDirectory)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        if (string.IsNullOrWhiteSpace(executableDirectory))
        {
            throw new ArgumentException(
                "Katalog aplikacji jest wymagany.",
                nameof(executableDirectory));
        }

        if (arguments.Any(argument => string.Equals(
                argument,
                PortableArgument,
                StringComparison.OrdinalIgnoreCase)))
        {
            return ApplicationMode.Portable;
        }

        string markerPath = Path.Combine(
            executableDirectory,
            PortableMarkerFileName);

        return File.Exists(markerPath)
            ? ApplicationMode.Portable
            : ApplicationMode.Standard;
    }
}
