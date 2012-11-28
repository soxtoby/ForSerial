using System;
using System.Collections.Generic;
using System.Linq;

namespace ForSerial.Objects
{
    public class ObjectReader
    {
        private readonly Writer writer;
        private readonly Dictionary<object, int> stuctureReferences = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance);
        private int referenceCount;

        private ObjectReader(Writer writer, ObjectParsingOptions options)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (options == null) throw new ArgumentNullException("options");

            Options = options;
            this.writer = writer;
        }

        public ObjectParsingOptions Options { get; private set; }

        public static void Read(object obj, Writer writer, ObjectParsingOptions options = null)
        {
            ObjectReader reader = new ObjectReader(writer, options ?? new ObjectParsingOptions());

            try
            {
                reader.Read(obj);
            }
            catch (Exception e)
            {
                throw new ObjectReadException(reader, e);
            }
        }

        private void Read(object input)
        {
            TypeDefinition typeDef = TypeCache.GetTypeDefinition(input);
            typeDef.Read(input, this, writer, new PartialOptions { SerializeTypeInformation = Options.SerializeTypeInformation });
        }

        public bool ReferenceStructure(object obj)
        {
            if (stuctureReferences.ContainsKey(obj))
            {
                writer.WriteReference(stuctureReferences[obj]);
                return true;
            }

            stuctureReferences[obj] = referenceCount;
            AddReference();
            return false;
        }

        public void AddReference()
        {
            referenceCount++;
        }

        public readonly Stack<PropertyDefinition> PropertyStack = new Stack<PropertyDefinition>();

        internal class ObjectReadException : Exception
        {
            public ObjectReadException(ObjectReader reader, Exception innerException)
                : base(BuildMessage(reader, innerException), innerException)
            {
            }

            private static string BuildMessage(ObjectReader reader, Exception innerException)
            {
                string propertyStack = reader.PropertyStack
                    .Select(p => p.FullName)
                    .Join(Environment.NewLine);
                return "{1}{0}At: {2}".FormatWith(Environment.NewLine, innerException.Message, propertyStack);
            }
        }
    }
}

