using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// Base class for markup extensions that build and manage a real <see cref="Binding"/> from the usual
/// binding properties, so a derived extension can wrap a binding with extra behaviour (such as a timer).
/// </summary>
public abstract class BindingUpdatableMarkupExtension : TargetUpdatableMarkupExtension
{
    /// <summary>The converter applied to the binding.</summary>
    public IValueConverter? Converter { get; set; }

    /// <summary>The culture passed to the converter.</summary>
    [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
    public CultureInfo? ConverterCulture { get; set; }

    /// <summary>The parameter passed to the converter.</summary>
    public object? ConverterParameter { get; set; }

    /// <summary>The name of the element to bind to.</summary>
    public string? ElementName { get; set; }

    /// <summary>The source property path.</summary>
    [ConstructorArgument("path")]
    public PropertyPath? Path { get; set; }

    /// <summary>The relative source for the binding.</summary>
    public RelativeSource? RelativeSource { get; set; }

    /// <summary>An explicit binding source.</summary>
    public object? Source { get; set; }

    /// <summary>The string format applied to the bound value.</summary>
    public string? StringFormat { get; set; }

    /// <summary>Builds a <see cref="Binding"/> from the configured properties.</summary>
    protected Binding MakeBinding()
    {
        var binding = new Binding
        {
            Converter = Converter,
            ConverterCulture = ConverterCulture,
            ConverterParameter = ConverterParameter,
            StringFormat = StringFormat,
        };

        if (Path is not null) binding.Path = Path;
        if (ElementName is not null) binding.ElementName = ElementName;
        if (RelativeSource is not null) binding.RelativeSource = RelativeSource;
        if (Source is not null) binding.Source = Source;

        return binding;
    }
}
