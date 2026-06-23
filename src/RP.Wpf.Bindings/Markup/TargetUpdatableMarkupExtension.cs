using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace RP.Wpf.Bindings.Markup;

/// <summary>
/// Base class for markup extensions that push fresh values into their target property <em>after</em>
/// the initial evaluation — for example on a timer or in response to an external event.
/// </summary>
/// <remarks>
/// A plain <see cref="MarkupExtension"/> produces a value once, at parse time, and is then forgotten.
/// To keep updating the target we must remember which object/property the extension was applied to.
/// This base records every target, then writes to each via <see cref="SetTarget"/>, marshalling back
/// to the owning dispatcher thread where necessary. It also handles the templated case, where the real
/// target isn't known at parse time, by returning itself for the shared <c>System.Windows.SharedDp</c>.
/// <para>After Thomas Levesque's "updatable markup extension" pattern.</para>
/// </remarks>
public abstract class TargetUpdatableMarkupExtension : MarkupExtension
{
    private readonly List<object> _targetObjects = new();

    /// <summary>The objects the extension has been applied to.</summary>
    protected IReadOnlyList<object> TargetObjects => _targetObjects;

    /// <summary>The target property — a <see cref="DependencyProperty"/> or a <see cref="PropertyInfo"/>.</summary>
    protected object? TargetProperty { get; private set; }

    /// <summary>
    /// Records the target and delegates value production to <see cref="ProvideValueInternal"/>.
    /// Sealed: derived classes override <see cref="ProvideValueInternal"/> instead.
    /// </summary>
    public sealed override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget target &&
            target.TargetObject is not null)
        {
            // Inside a template the real target isn't known yet; defer by returning the extension.
            if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
                return this;

            if (!_targetObjects.Contains(target.TargetObject))
                _targetObjects.Add(target.TargetObject);

            TargetProperty = target.TargetProperty;
        }

        return ProvideValueInternal(serviceProvider);
    }

    /// <summary>Produces the initial value for the target property.</summary>
    protected abstract object ProvideValueInternal(IServiceProvider serviceProvider);

    /// <summary>
    /// Writes <paramref name="value"/> to every recorded target, dispatching to the owning thread for
    /// dependency-property targets when called from another thread.
    /// </summary>
    protected virtual void SetTarget(object? value)
    {
        foreach (var target in _targetObjects)
        {
            if (TargetProperty is DependencyProperty dependencyProperty && target is DependencyObject dependencyObject)
            {
                if (dependencyObject.CheckAccess())
                    dependencyObject.SetValue(dependencyProperty, value);
                else
                    dependencyObject.Dispatcher.Invoke(() => dependencyObject.SetValue(dependencyProperty, value));
            }
            else if (TargetProperty is PropertyInfo property)
            {
                property.SetValue(target, value, null);
            }
        }
    }
}
