using System.Windows;
using System.Windows.Threading;

namespace RP.Wpf.Bindings.Demo;

/// <summary>
/// Interaction logic for App.xaml. Shows the window normally, or — with <c>--capture &lt;path&gt;</c> —
/// renders one frame to a PNG off-screen and exits, so the README screenshot can be produced headlessly.
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var window = new MainWindow();
        var capturePath = ScreenCapture.PathFrom(e.Args);

        if (capturePath is null)
        {
            window.Show();
            return;
        }

        window.WindowStartupLocation = WindowStartupLocation.Manual;
        window.Left = -10000;
        window.Top = -10000;
        window.ShowInTaskbar = false;
        window.Show();

        // A little longer so the live clock and network flag have ticked at least once.
        var settle = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(900) };
        settle.Tick += (_, _) =>
        {
            settle.Stop();
            ScreenCapture.SavePng(window, capturePath);
            Shutdown();
        };
        settle.Start();
    }
}
