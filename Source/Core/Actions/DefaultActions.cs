namespace Proxoft.Redux.Core.Actions;

public static class DefaultActions
{
    public static readonly IAction None = new NoneAction();
    public static readonly IAction Initialize = new InitializeAction();
    public static readonly IAction InitializeEffects = new InitializeEffectsAction();
}
