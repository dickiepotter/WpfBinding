using System.Windows.Threading;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// Base class for extensions that refresh their target by polling a source value on a
/// background-priority timer, pushing a new value only when it differs from the last.
/// </summary>
/// <typeparam name="T">The polled value type.</typeparam>
public abstract class TimedUpdatableMarkupExtension<T> : TargetUpdatableMarkupExtension
{
    private static readonly TimeSpan DefaultDelay = TimeSpan.FromMilliseconds(500);

    private DispatcherTimer? _timer;
    private TimeSpan _delay = DefaultDelay;
    private T? _lastValue;

    /// <summary>The interval between polls. Defaults to 500&#160;ms.</summary>
    public TimeSpan Delay
    {
        get => _delay;
        set
        {
            _delay = value;
            ResetTimer();
        }
    }

    /// <summary>Reads the current source value. Called on every poll.</summary>
    protected abstract T GetSource();

    protected override object ProvideValueInternal(IServiceProvider serviceProvider)
    {
        ResetTimer();
        _timer!.Start();
        return GetSource()!;
    }

    private void ResetTimer()
    {
        var wasRunning = _timer?.IsEnabled ?? false;
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
        }

        _timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = _delay };
        _timer.Tick += OnTick;

        Update();

        if (wasRunning)
            _timer.Start();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        _timer!.Stop();
        Update();
        _timer.Start();
    }

    private void Update()
    {
        var newValue = GetSource();
        if (!EqualityComparer<T>.Default.Equals(newValue, _lastValue))
            SetTarget(newValue);

        _lastValue = newValue;
    }
}
