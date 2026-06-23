namespace RP.Wpf.Bindings.Demo;

/// <summary>
/// A deliberately "dumb" source for the <c>TimedBinding</c> demo: it exposes an instance property that
/// changes constantly but raises <b>no</b> change notification. A normal <see cref="System.Windows.Data.Binding"/>
/// would read <see cref="Now"/> once and never again; <c>TimedBinding</c> is what keeps it current.
/// </summary>
public sealed class Clock
{
    /// <summary>The current time, re-evaluated every time it is read.</summary>
    public DateTime Now => DateTime.Now;
}
