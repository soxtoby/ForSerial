using System;

namespace json
{
    public abstract class Reader
    {
        protected readonly StateStack<Writer> writer;

        protected Reader(Writer baseWriter)
        {
            writer = new StateStack<Writer>(baseWriter);
        }

        public abstract OutputStructure ReadSubStructure(Writer subWriter);

        protected IDisposable UseObjectPropertyContext(OutputStructure propertyOwner, string propertyName)
        {
            return writer.OverrideState(new PropertyContextWriter(writer.Base, propertyOwner, propertyName));
        }

        protected IDisposable UseArrayContext(SequenceOutput array)
        {
            return writer.OverrideState(new SequenceContextWriter(writer.Base, array));
        }
    }
}
