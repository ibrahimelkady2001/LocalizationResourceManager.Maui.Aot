using LocalizationResourceManager.Maui.ComponentModel;

namespace LocalizationResourceManager.Maui;

/// <summary>
/// Localized string, updated by tracking current culture from current localizarion resource manager.
/// </summary>
public class LocalizedString : ObservableObject
{
    private readonly Func<string> generator;
    private static int _diagCount = 0;

    public LocalizedString(Func<string> generator)
        : this(LocalizationResourceManager.Current, generator)
    {
    }

    public LocalizedString(LocalizationResourceManager localizationManager, Func<string> generator)
    {
        this.generator = generator;

        // This instance will be unsubscribed and GCed if no one references it
        // since LocalizationResourceManager uses WeekEventManger
        localizationManager.PropertyChanged += (sender, e) => OnPropertyChanged((string?)null);
    }

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public string Localized
    {
        get
        {
            try
            {
                var result = generator();
                var n = System.Threading.Interlocked.Increment(ref _diagCount);
                if (n <= 5)
                {
                    var msg = $"[LocalizedString#{n}] result='{result ?? "<null>"}'";
                    System.Diagnostics.Debug.WriteLine(msg);
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try { await Clipboard.SetTextAsync(msg); } catch { }
                    });
                }
                return result ?? string.Empty;
            }
            catch (Exception ex)
            {
                var msg = $"[LocalizedString.Localized] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(msg);
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try { await Clipboard.SetTextAsync(msg); } catch { }
                });
                return $"[ERR:{ex.GetType().Name}]";
            }
        }
    }

    [Microsoft.Maui.Controls.Internals.Preserve(Conditional = true)]
    public static implicit operator LocalizedString(Func<string> func) => new(func);
}