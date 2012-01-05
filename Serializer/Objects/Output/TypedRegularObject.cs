using System;

namespace json.Objects
{
    internal class TypedRegularObject : TypedObjectBase
    {
        private readonly object obj;
        public override object Object { get { return obj; } }

        public TypedRegularObject(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
            obj = Activator.CreateInstance(typeDef.Type);
        }

        public TypedRegularObject(object obj)
        {
            if (obj != null)
            {
                TypeDef = CurrentTypeHandler.GetTypeDefinition(obj.GetType());
                this.obj = obj;
            }
        }

        public override void AddProperty(string name, TypedValue value)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
                value.AssignToProperty(Object, property);
        }
    }
}
