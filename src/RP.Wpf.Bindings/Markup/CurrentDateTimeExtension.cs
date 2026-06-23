using System.Windows.Markup;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// A markup extension that binds a live, self-updating current date/time string, formatted with
/// <see cref="StringFormat"/> and refreshed on the inherited polling timer.
/// </summary>
/// <example><c>Text="{rp:CurrentDateTime 'HH:mm:ss'}"</c></example>
[MarkupExtensionReturnType(typeof(string))]
public sealed class CurrentDateTimeExtension : TimedUpdatableMarkupExtension<string>
{
    /// <summary>Creates the extension with no preset format.</summary>
    public CurrentDateTimeExtension()
    {
    }

    /// <summary>Creates the extension using <paramref name="stringFormat"/>.</summary>
    public CurrentDateTimeExtension(string? stringFormat) => StringFormat = stringFormat;

    /// <summary>The <see cref="DateTime"/> format string.</summary>
    [ConstructorArgument("stringFormat")]
    public string? StringFormat { get; set; }

    /// <summary>The format provider used to render the value.</summary>
    public IFormatProvider? FormatProvider { get; set; }

    /// <inheritdoc/>
    protected override string GetSource() => DateTime.Now.ToString(StringFormat, FormatProvider);
}
