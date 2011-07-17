using System;
using System.Collections.Generic;

namespace json
{
    public abstract class Parser
    {
        private readonly ParseValueFactory baseValueFactory;
        private readonly Stack<ParseValueFactory> contextValueFactories = new Stack<ParseValueFactory>();
        protected ParseValueFactory ValueFactory { get { return contextValueFactories.Peek(); } }

        protected Parser(ParseValueFactory baseValueFactory)
        {
            this.baseValueFactory = baseValueFactory;
            contextValueFactories.Push(baseValueFactory);
        }

        public abstract ParseObject ParseSubObject(ParseValueFactory subParseValueFactory);

        protected void UsingObjectPropertyContext(ParseObject propertyOwner, string propertyName, Action action)
        {
            UsingValueFactoryContext(new PropertyValueFactory(baseValueFactory, propertyOwner, propertyName), action);
        }

        protected void UsingArrayContext(ParseArray array, Action action)
        {
            UsingValueFactoryContext(new ArrayValueFactory(baseValueFactory, array), action);
        }

        protected void UsingValueFactoryContext(ParseValueFactory contextValueFactory, Action action)
        {
            contextValueFactories.Push(contextValueFactory);
            action();
            contextValueFactories.Pop();
        }
    }
}