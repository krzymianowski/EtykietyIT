namespace EtykietyIT.Forms;

internal static class ApplicationIconProvider
{
    private static readonly object SyncRoot = new();

    private static Icon? _icon;

    public static void Initialize()
    {
        lock (SyncRoot)
        {
            if (_icon is not null)
            {
                return;
            }

            _icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ??
                throw new InvalidOperationException(
                    "Nie można odczytać ikony osadzonej w pliku aplikacji.");
        }
    }

    public static void Apply(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        Initialize();
        form.Icon = _icon;
    }

    public static void Shutdown()
    {
        lock (SyncRoot)
        {
            _icon?.Dispose();
            _icon = null;
        }
    }
}
