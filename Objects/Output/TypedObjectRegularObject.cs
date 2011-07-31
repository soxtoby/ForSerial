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
                TypeDef = TypeDefinition.GetTypeDefinition(obj.GetType());
                this.obj = obj;
            }
        }

        public override void AddObject(string name, TypedObjectObject value)
        {
            TypedObjectObject objectValue = TypedObjectObject.GetObjectAsTypedObjectObject(value);

            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
                objectValue.AssignToProperty(Object, property);
        }

        public override void AddArray(string name, TypedObjectArray array)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
            {
                if (property.CanSet)
                    SetArrayProperty(property, array);
                else if (property.CanGet)
                    PopulateArrayProperty(property, array);
            }
        }

        private void SetArrayProperty(PropertyDefinition property, TypedObjectArray array)
        {
            property.SetOn(Object, array.GetTypedArray());
        }

        private void PopulateArrayProperty(PropertyDefinition property, TypedObjectArray array)
        {
            array.PopulateCollection(property.GetFrom(Object));
        }

        public override void AddProperty(string name, object value)
        {
            PropertyDefinition property = TypeDef.Properties.Get(name);
            if (property != null)
                property.SetOn(Object, value);
        }

        public void PreBuild(PreBuildInfo preBuildInfo, Parser parser)
        {
            preBuildInfo.PreBuild(Object, parser, new TypedObjectBuilder.TypedObjectSubBuilder(Object));
        }
    }
}
