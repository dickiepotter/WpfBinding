using System.Windows.Markup;

// Expose both namespaces under one tidy XML namespace so consumers can write a single xmlns and reach
// every extension and converter — e.g. xmlns:rp="https://schemas.richardpotter.dev/wpf".
[assembly: XmlnsDefinition("https://schemas.richardpotter.dev/wpf", "RP.Wpf.Bindings.Markup")]
[assembly: XmlnsDefinition("https://schemas.richardpotter.dev/wpf", "RP.Wpf.Bindings.Converters")]
[assembly: XmlnsPrefix("https://schemas.richardpotter.dev/wpf", "rp")]
