namespace Proxoft.Redux.Core;

public sealed record StateActionPair<T>(T State, IAction Action);