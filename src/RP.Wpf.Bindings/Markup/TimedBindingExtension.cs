using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// A markup extension that provides a read-only binding whose target is refreshed on a
/// background-priority timer — useful for surfacing a source that changes <em>without</em> raising
/// <see cref="System.ComponentModel.INotifyPropertyChanged"/> (for example <c>DateTime.Now</c>).
/// </summary>
/// <remarks>
/// The binding is forced to <see cref="BindingMode.OneWay"/>; the timer periodically calls
/// <see cref="BindingExpressionBase.UpdateTarget"/> on each active target to re-pull the source.
/// </remarks>
public sealed class TimedBindingExtension : BindingUpdatableMarkupExtension
{
    private static readonly TimeSpan DefaultDelay = TimeSpan.FromMilliseconds(500);

    private DispatcherTimer _timer;
    private TimeSpan _delay = DefaultDelay;

    /// <summary>Creates the extension with the default 500&#160;ms refresh interval.</summary>
    public TimedBindingExtension() => _timer = CreateTimer();

    /// <summary>The interval between target refreshes. Defaults to 500&#160;ms.</summary>
    public TimeSpan Delay
    {
        get => _delay;
        set
        {
            _delay = value;
            var wasRunning = _timer.IsEnabled;
            _timer.Stop();
            _timer.Tick -= OnTick;
            _timer = CreateTimer();
            if (wasRunning)
                _timer.Start();
        }
    }

    /// <inheritdoc/>
    protected override object ProvideValueInternal(IServiceProvider serviceProvider)
    {
        var binding = MakeBinding();
        binding.Mode = BindingMode.OneWay;

        var value = binding.ProvideValue(serviceProvider);
        _timer.Start();
        return value;
    }

    private DispatcherTimer CreateTimer()
    {
        var created = new DispatcherTimer(DispatcherPriority.Background) { Interval = _delay };
        created.Tick += OnTick;
        return created;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _timer.Stop();
        RefreshTargets();
        _timer.Start();
    }

    private void RefreshTargets()
    {
        if (TargetProperty is not DependencyProperty dependencyProperty)
            return;

        foreach (var target in TargetObjects)
        {
            if (target is not DependencyObject dependencyObject)
                continue;

            var expression = BindingOperations.GetBindingExpressionBase(dependencyObject, dependencyProperty);
            if (expression is { Status: BindingStatus.Active })
                expression.UpdateTarget();
        }
    }
}
