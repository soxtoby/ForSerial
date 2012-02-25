using System;

namespace json.Objects
{
    public class TypedObjectOutputStructure : OutputStructureBase, TypedValue
    {
        private TypedObject typedObject;

        public TypedObjectOutputStructure()
        {
        }

        public TypedObjectOutputStructure(object obj)
        {
            typedObject = new TypedRegularObject(obj);
        }

        public TypedObjectOutputStructure(TypeDefinition typeDef)
        {
            SetType(typeDef);
        }

        public object Object
        {
            get { return typedObject.Object; }
        }

        public override bool SetType(string typeIdentifier, Reader reader)
        {
            TypeDefinition typeDef = CurrentTypeHandler.GetTypeDefinition(typeIdentifier);
            bool useCurrentType = CurrentTypeIsNotCompatible(typeDef);
            if (useCurrentType)
                typeDef = typedObject.TypeDef;
            else
                SetType(typeDef);

            return PreBuild(reader, typeDef);
        }

        private bool PreBuild(Reader reader, TypeDefinition typeDef)
        {
            PreBuildInfo preBuildInfo = typeDef.GetPreBuildInfo(reader);
            if (preBuildInfo == null) return false;

            preBuildInfo.PreBuild(Object, reader, new TypedObjectBuilder.TypedObjectSubBuilder(Object));
            return true;
        }

        private bool CurrentTypeIsNotCompatible(TypeDefinition typeDef)
        {
            return typedObject != null && !typeDef.Type.CanBeCastTo(typedObject.TypeDef.Type);
        }

        private void SetType(TypeDefinition typeDef)
        {
            typedObject = typeDef.CreateStructure();
        }

        public void AddProperty(string name, TypedValue value)
        {
            AssertObjectInitialized();
            typedObject.AddProperty(name, value);
        }

        public override Output CreateValue(string name, Writer valueFactory, object value)
        {
            AssertObjectInitialized();
            return typedObject.CreateValue(name, value);
        }

        public override OutputStructure BeginStructure(string name, Writer valueFactory)
        {
            AssertObjectInitialized();
            return typedObject.BeginStructure(name);
        }

        public override void EndStructure(Writer baseFactory)
        {
            typedObject.EndStructure();
        }

        public override void EndSequence(Writer baseFactory)
        {
            typedObject.EndSequence();
        }

        public override SequenceOutput BeginSequence(string name, Writer valueFactory)
        {
            AssertObjectInitialized();
            return typedObject.BeginSequence(name);
        }

        private void AssertObjectInitialized()
        {
            if (typedObject == null)
                throw new ObjectNotInitialized();
        }

        public void AssignToProperty(object owner, PropertyDefinition property)
        {
            typedObject.AssignToProperty(owner, property);
        }

        public object GetTypedValue()
        {
            return Object;
        }

        internal class ObjectNotInitialized : Exception
        {
            public ObjectNotInitialized() : base("Tried to add a property to an uninitialized object. Make sure input contains type information.") { }
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
            ((TypedObjectOutputStructure)structure).AddProperty(name, this);
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
            ((TypedSequence)sequence).AddItem(typedObject.Object);
        }
    }
}
