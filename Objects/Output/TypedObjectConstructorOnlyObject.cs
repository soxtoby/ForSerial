using System.Collections.Generic;

namespace json.Objects
{
    internal class TypedObjectConstructorOnlyObject : TypedObjectParseObjectBase
    {
        private readonly Dictionary<string, object> properties = new Dictionary<string, object>();

        public TypedObjectConstructorOnlyObject(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
        }

        public override object Object
        {
            get { throw new System.NotImplementedException(); }
        }

        public override void AddProperty(string name, object value)
        {
            properties[name] = value;
        }

        public override void AddObject(string name, TypedObjectObject value)
        {
            properties[name] = value.Object;
        }

        public override void AddArray(string name, TypedObjectArray array)
        {
            properties[name] = array.GetTypedArray();
        }
    }
}