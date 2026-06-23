using System.Globalization;
using System.Windows.Data;

namespace RP.Wpf.Bindings.Converters;

/// <summary>
/// Converts a value to the simple name of its runtime type (handy for a diagnostic column), or
/// <see langword="null"/> when the value is <see langword="null"/>.
/// </summary>
[ValueConversion(typeof(object), typeof(string))]
public sealed class ObjectToTypeStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.GetType().Name;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException($"{nameof(ObjectToTypeStringConverter)} is a one-way converter.");
}
