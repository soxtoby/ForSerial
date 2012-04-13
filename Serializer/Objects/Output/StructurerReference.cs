namespace ForSerial.Objects
{
    internal class StructurerReference : ObjectOutput
    {
        private readonly ObjectContainer referencedOutput;

        public StructurerReference(ObjectContainer referencedOutput)
        {
            this.referencedOutput = referencedOutput;
        }

        public TypeDefinition TypeDef { get { return referencedOutput.TypeDef; } }

        public object GetTypedValue()
        {
            return referencedOutput.GetTypedValue();
        }

        public void AssignToProperty(object obj, PropertyDefinition property)
        {
            referencedOutput.AssignToProperty(obj, property);
        }
    }
}