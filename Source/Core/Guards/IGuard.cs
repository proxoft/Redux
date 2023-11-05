namespace Proxoft.Redux.Core.Guards;

public interface IGuard<TState>
{
    /// <summary>
    /// Validates whether the dispatched action can be reduced.
    /// This method can
    /// - suppress the reduce of an action by returning NoneAction (the pipeline will ignore NoneAction)
    /// - replace dispatched action by a new one (e.g. Alarm, Error, etc.)
    /// </summary>
    /// <param name="action">Action that has been dispatched and is about to be reduced.</param>
    /// <param name="state">Current state</param>
    /// <returns>Action that will really be reduced.
    /// If the guard validates current action as ok, it is expected to return same instance.
    /// </returns>
    IAction Validate(IAction action, TState state);
}