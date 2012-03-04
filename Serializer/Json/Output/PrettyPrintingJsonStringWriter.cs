using System;
using System.IO;

namespace json.Json
{
    public class PrettyPrintingJsonStringWriter : JsonStringWriter
    {
        private bool isRoot = true;
        private readonly string indentation;
        private int indentationLevel;
        private bool isBeginningOfContainer;
        private bool isPropertyValue;

        public PrettyPrintingJsonStringWriter(TextWriter textWriter, string indentation = null)
            : base(textWriter)
        {
            this.indentation = indentation ?? "  ";
        }

        public override void BeginStructure(Type readerType)
        {
            base.BeginStructure(readerType);
            isBeginningOfContainer = true;
        }

        public override void BeginSequence()
        {
            base.BeginSequence();
            isBeginningOfContainer = true;
        }

        public override void EndStructure()
        {
            if (isBeginningOfContainer)
            {
                Json.Write(' ');
                isBeginningOfContainer = false;
            }
            else
            {
                indentationLevel--;
                MoveToNextLine();
            }
            base.EndStructure();
        }

        public override void WriteReference(int referenceIndex)
        {
            Json.Write(@"{ ""_ref"": ");
            Json.Write(referenceIndex);
            Json.Write(" }");
        }

        public override void AddProperty(string name)
        {
            base.AddProperty(name);
            Json.Write(' ');
            isPropertyValue = true;
        }

        public override void EndSequence()
        {
            if (isBeginningOfContainer)
            {
                Json.Write(' ');
                isBeginningOfContainer = false;
            }
            else
            {
                indentationLevel--;
                MoveToNextLine();
            }
            base.EndSequence();
        }

        protected override void Delimit()
        {
            base.Delimit();

            if (isBeginningOfContainer)
                indentationLevel++;

            if (!isRoot && !isPropertyValue)
                MoveToNextLine();

            isRoot = false;
            isBeginningOfContainer = false;
            isPropertyValue = false;
        }

        private void MoveToNextLine()
        {
            Json.WriteLine();
            for (int i = 0; i < indentationLevel; i++)
                Json.Write(indentation);
        }
    }
}