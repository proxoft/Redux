namespace Proxoft.Redux.Core
{
    public class StateActionPair<T>
    {
        public StateActionPair(T state, IAction action)
        {
            this.State = state;
            this.Action = action;
        }

        public T State { get; }
        public IAction Action { get; }
    }
}
