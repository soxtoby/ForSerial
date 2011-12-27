using System;

namespace json.Objects
{
    internal class TypedObjectRegularObject : TypedObjectParseObjectBase
    {
        private readonly object obj;
        public override object Object { get { return obj; } }

        public TypedObjectRegularObject(TypeDefinition typeDef)
        {
            TypeDef = typeDef;
            obj = Activator.CreateInstance(typeDef.Type);
        }

        public TypedObjectRegularObject(object obj)
        {
            if (obj != null)
            {
                TypeDef = CurrentTypeHandler.GetTypeDefinition(obj.GetType());
                this.obj = obj;
            }
        }

        public override void AddProperty(string name, TypedObjectValue value)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
                value.AssignToProperty(Object, property);
        }

        public void PreBuild(PreBuildInfo preBuildInfo, Parser parser)
        {
            preBuildInfo.PreBuild(Object, parser, new TypedObjectBuilder.TypedObjectSubBuilder(Object));
        }
    }
}
