namespace LocalizationResourceManager.Maui;

/// <summary>
/// Markup extension (XAML) for handling and updating localized string by tracking current culture from current localization resource manager.
/// </summary>
[ContentProperty(nameof(Text))]
[AcceptEmptyServiceProvider]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    private string text = string.Empty;

    public string Text
    {
        get => text;
        set => text = LocalizationResourceManager.Current.IsNameWithDotsSupported
            ? value.Replace(".", LocalizationResourceManager.Current.DotSubstitution)
            : value;
    }

    public string? StringFormat { get; set; }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        // NativeAOT fix: string Path-based bindings silently fail under NativeAOT because
        // MAUI's binding engine no longer uses reflection for path resolution in trimmed
        // builds. Path="." binds the source object itself (zero property-name reflection).
        // The converter does the key lookup directly — no reflection required at runtime.
        // LocalizationResourceManager fires PropertyChanged on culture change, so the
        // binding re-evaluates automatically.
        var capturedText   = Text;
        var capturedFormat = StringFormat;

        return new Binding
        {
            Mode      = BindingMode.OneWay,
            Path      = ".",
            Source    = LocalizationResourceManager.Current,
            Converter = new LocalizationConverter(capturedText, capturedFormat)
        };
    }

    private sealed class LocalizationConverter : IValueConverter
    {
        private readonly string  _key;
        private readonly string? _format;

        public LocalizationConverter(string key, string? format)
        {
            _key    = key;
            _format = format;
        }

        public object? Convert(object? value, Type targetType, object? parameter,
                               System.Globalization.CultureInfo culture)
        {
            var result = LocalizationResourceManager.Current[_key];
            return _format != null ? string.Format(_format, result) : result;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter,
                                   System.Globalization.CultureInfo culture)
            => throw new NotSupportedException();
    }
}