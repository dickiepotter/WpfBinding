using System.Windows;

namespace RP.Wpf.Bindings.Demo;

/// <summary>
/// The demo window is intentionally code-free: every live value and conversion on it is driven
/// entirely from XAML by the markup extensions and converters in <c>RP.Wpf.Bindings</c>.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();
}
