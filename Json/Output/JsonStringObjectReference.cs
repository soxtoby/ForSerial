using System;

namespace json.Json
{
    public partial class JsonStringBuilder
    {
        private class JsonStringObjectReference : ParseObjectBase
        {
            private readonly uint referenceId;

            public JsonStringObjectReference(uint referenceId)
            {
                this.referenceId = referenceId;
            }

            public override string ToString()
            {
                return @"{""_ref"":" + referenceId + "}";
            }

            public override void AddNull(string name)
            {
                throw new CannotAddValueToReference();
            }

            public override void AddBoolean(string name, bool value)
            {
                throw new CannotAddValueToReference();
            }

            public override void AddNumber(string name, double value)
            {
                throw new CannotAddValueToReference();
            }

            public override void AddString(string name, string value)
            {
                throw new CannotAddValueToReference();
            }

            public override void AddObject(string name, ParseObject value)
            {
                throw new CannotAddValueToReference();
            }

            public override void AddArray(string name, ParseArray value)
            {
                throw new CannotAddValueToReference();
            }
        }

        internal class CannotAddValueToReference : Exception
        {
            public CannotAddValueToReference() : base("Parser should not be trying to add properties to a reference object.") { }
        }
    }
}