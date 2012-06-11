using System;
using System.Collections.Generic;
using System.Linq;

namespace ForSerial.Objects
{
    public class ObjectReader
    {
        private readonly Writer writer;
        private readonly ObjectParsingOptions options;
        private readonly Dictionary<object, int> stuctureReferences = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Instance);

        private ObjectReader(Writer writer, ObjectParsingOptions options)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            if (options == null) throw new ArgumentNullException("options");

            this.writer = writer;
            this.options = options;
        }

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
            Read(input, options.SerializeTypeInformation != TypeInformationLevel.None);
        }

        private void Read(object input, bool requestTypeIdentification)
        {
            TypeDefinition typeDef = TypeCache.GetTypeDefinition(input);
            typeDef.Read(input, this, writer, requestTypeIdentification);
        }

        public bool ShouldWriteTypeIdentification(bool typeIdentifierRequested)
        {
            return options.SerializeTypeInformation == TypeInformationLevel.All
                || options.SerializeTypeInformation == TypeInformationLevel.Minimal && typeIdentifierRequested;
        }

        public bool ReferenceStructure(object obj)
        {
            if (!options.MaintainReferences)
                return false;

            if (stuctureReferences.ContainsKey(obj))
            {
                writer.WriteReference(stuctureReferences[obj]);
                return true;
            }

            stuctureReferences[obj] = stuctureReferences.Count;
            return false;
        }

        public readonly Stack<PropertyDefinition> PropertyStack = new Stack<PropertyDefinition>();

        public MemberAccessibility MemberAccessibility
        {
            get { return options.MemberAccessibility; }
        }

        public MemberType MemberType
        {
            get { return options.MemberType; }
        }

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

