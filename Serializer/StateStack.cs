using System;
using System.Collections.Generic;

namespace json
{
    public class StateStack<TState>
    {
        private readonly Stack<TState> stack = new Stack<TState>();
        public TState Base { get; private set; }

        public StateStack(TState baseState)
        {
            Base = baseState;
            stack.Push(baseState);
        }

        public IDisposable OverrideState(TState state)
        {
            return new StateOverride<TState>(stack, state);
        }

        public TState Current
        {
            get { return stack.Peek(); }
        }

        private class StateOverride<TOverride> : IDisposable
        {
            private readonly Stack<TOverride> stack;

            public StateOverride(Stack<TOverride> stack, TOverride overrideState)
            {
                this.stack = stack;
                stack.Push(overrideState);
            }

            public void Dispose()
            {
                stack.Pop();
            }
        }
    }
}
