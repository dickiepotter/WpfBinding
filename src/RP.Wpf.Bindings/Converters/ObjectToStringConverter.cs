using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace RP.Wpf.Bindings.Converters;

/// <summary>
/// Formats an object as a string, honouring <see cref="StringFormat"/> for <see cref="IFormattable"/>
/// values. A collection is rendered as one formatted line per item.
/// </summary>
/// <remarks>
/// Rarely needed now that <c>Binding.StringFormat</c> exists, but retained for the
/// collection-joining behaviour and for use outside a binding. (It also fixes the original, which
/// formatted the whole collection on every line instead of each item.)
/// <para>
/// It is both an <see cref="IValueConverter"/> and a <see cref="MarkupExtension"/>: deriving from
/// <see cref="MarkupExtension"/> and returning itself lets it be used inline with a constructor
/// argument — <c>Converter={rp:ObjectToStringConverter 'HH:mm:ss'}</c> — with no resource declaration.
/// </para>
/// </remarks>
[ContentProperty(nameof(StringFormat))]
[ValueConversion(typeof(object), typeof(string))]
public sealed class ObjectToStringConverter : MarkupExtension, IValueConverter
{
    public ObjectToStringConverter()
    {
    }

    public ObjectToStringConverter(string? stringFormat) => StringFormat = stringFormat;

    /// <summary>Returns the converter itself, so it can be used as an inline markup extension.</summary>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    /// <summary>The format string applied to <see cref="IFormattable"/> values.</summary>
    public string? StringFormat { get; set; }

    /// <summary>The format provider applied when formatting.</summary>
    public IFormatProvider? FormatProvider { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        // A string is itself IEnumerable; treat it as a single value, not a sequence of characters.
        if (value is IEnumerable sequence and not string)
        {
            var builder = new StringBuilder();
            foreach (var item in sequence)
                builder.AppendLine(Format(item));

            if (builder.Length >= Environment.NewLine.Length)
                builder.Length -= Environment.NewLine.Length;

            return builder.ToString();
        }

        return Format(value);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(ObjectToStringConverter)} is a one-way converter.");

    private string? Format(object? value) => value switch
    {
        null => null,
        string text => text,
        IFormattable formattable => formattable.ToString(StringFormat, FormatProvider),
        _ => value.ToString(),
    };
}
