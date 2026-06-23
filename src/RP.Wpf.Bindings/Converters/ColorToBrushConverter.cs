using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RP.Wpf.Bindings.Converters;

/// <summary>
/// Converts a colour to a <see cref="SolidColorBrush"/> and back, accepting either a WPF
/// <see cref="Color"/> or a <see cref="System.Drawing.Color"/> as the source.
/// </summary>
[ValueConversion(typeof(Color), typeof(SolidColorBrush))]
[ValueConversion(typeof(System.Drawing.Color), typeof(SolidColorBrush))]
public sealed class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            Color color => new SolidColorBrush(color),
            System.Drawing.Color drawing => new SolidColorBrush(Color.FromArgb(drawing.A, drawing.R, drawing.G, drawing.B)),
            _ => null,
        };

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SolidColorBrush brush)
            return null;

        if (targetType == typeof(Color))
            return brush.Color;

        if (targetType == typeof(System.Drawing.Color))
        {
            var c = brush.Color;
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        return null;
    }
}
