using System;

namespace json
{
    public abstract class Parser
    {
        protected readonly StateStack<ParseValueFactory> valueFactory;

        protected Parser(ParseValueFactory baseValueFactory)
        {
            valueFactory = new StateStack<ParseValueFactory>(baseValueFactory);
        }

        public abstract ParseObject ParseSubObject(ParseValueFactory subParseValueFactory);

        protected IDisposable UseObjectPropertyContext(ParseObject propertyOwner, string propertyName)
        {
            return valueFactory.OverrideState(new PropertyValueFactory(valueFactory.Base, propertyOwner, propertyName));
        }

        protected IDisposable UseArrayContext(ParseArray array)
        {
            return valueFactory.OverrideState(new ArrayValueFactory(valueFactory.Base, array));
        }
    }
}
