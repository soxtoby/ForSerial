using NUnit.Framework;

namespace json.Tests
{
    internal class TestOutput : Output
    {
        public void AddToStructure(OutputStructure structure, string name)
        {
        }

        public void AddToSequence(SequenceOutput sequence)
        {
        }

        public OutputStructure AsStructure()
        {
            return NullOutputStructure.Instance;
        }
    }

    internal class TestWriter : Writer
    {
        public virtual Output CreateValue(object value)
        {
            return new TestOutput();
        }

        public virtual OutputStructure BeginStructure()
        {
            return NullOutputStructure.Instance;
        }

        public virtual SequenceOutput BeginSequence()
        {
            return NullSequence.Instance;
        }

        public virtual OutputStructure CreateReference(OutputStructure outputStructure)
        {
            throw new AssertionException("CreateReference not implemented.");
        }

        public void EndStructure()
        {
        }

        public void EndSequence()
        {
        }
    }

    internal class SameReferenceTwice
    {
        public object One { get; set; }
        public object Two { get; set; }

        public SameReferenceTwice() { }

        public SameReferenceTwice(object obj)
        {
            One = Two = obj;
        }
    }

    internal class TwoReferencesTwice
    {
        public object One { get; set; }
        public object Two { get; set; }
        public object Three { get; set; }
        public object Four { get; set; }

        public TwoReferencesTwice() { }

        public TwoReferencesTwice(object odd, object even)
        {
            One = Three = odd;
            Two = Four = even;
        }
    }

    internal class WatchForReferenceBuilder : TestWriter
    {
        public OutputStructure ReferencedObject { get; private set; }

        public override OutputStructure CreateReference(OutputStructure outputStructure)
        {
            ReferencedObject = outputStructure;
            return outputStructure;
        }
    }

    internal class CustomCreateObject : NullOutputStructure
    {
        public override OutputStructure BeginStructure(string name, Writer valueFactory)
        {
            ((CustomCreateWriter)valueFactory).ObjectsCreatedFromProperties++;
            return base.BeginStructure(name, valueFactory);
        }

        public override SequenceOutput BeginSequence(string name, Writer valueFactory)
        {
            ((CustomCreateWriter)valueFactory).ArraysCreatedFromProperties++;
            return base.BeginSequence(name, valueFactory);
        }
    }

    internal class CustomCreateArray : NullSequence
    {
        public override OutputStructure BeginStructure(Writer valueFactory)
        {
            ((CustomCreateWriter)valueFactory).ObjectsCreatedFromArrays++;
            return base.BeginStructure(valueFactory);
        }

        public override SequenceOutput BeginSequence(Writer valueFactory)
        {
            ((CustomCreateWriter)valueFactory).ArraysCreatedFromArrays++;
            return base.BeginSequence(valueFactory);
        }
    }

    internal class CustomCreateWriter : TestWriter
    {
        public int ObjectsCreatedFromProperties { get; set; }
        public int ArraysCreatedFromProperties { get; set; }
        public int ObjectsCreatedFromArrays { get; set; }
        public int ArraysCreatedFromArrays { get; set; }

        public override OutputStructure BeginStructure()
        {
            return new CustomCreateObject();
        }

        public override SequenceOutput BeginSequence()
        {
            return new CustomCreateArray();
        }
    }
}