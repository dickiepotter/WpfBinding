using System.Net.NetworkInformation;
using System.Windows.Markup;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// A markup extension that binds a live <see cref="bool"/> indicating whether the machine currently
/// has network connectivity, updating the target whenever availability changes.
/// </summary>
/// <example><c>IsChecked="{rp:NetworkAvailable}"</c></example>
[MarkupExtensionReturnType(typeof(bool))]
public sealed class NetworkAvailableExtension : TargetUpdatableMarkupExtension
{
    /// <summary>Creates the extension and subscribes to network-availability changes.</summary>
    public NetworkAvailableExtension() =>
        NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

    private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e) =>
        SetTarget(e.IsAvailable);

    /// <inheritdoc/>
    protected override object ProvideValueInternal(IServiceProvider serviceProvider) =>
        NetworkInterface.GetIsNetworkAvailable();
}
