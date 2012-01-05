
namespace json
{
    /// <summary>
    /// Ignores all inputs. Do not derive from this type unless you need a Null Object
    /// with extra methods, or are using the class for testing.
    /// </summary>
    public class NullOutputStructure : OutputStructureBase
    {
        private static NullOutputStructure instance;
        public static NullOutputStructure Instance
        {
            get { return instance ?? (instance = new NullOutputStructure()); }
        }

        protected NullOutputStructure()
        {
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
        }
    }

    /// <summary>
    /// Ignores all inputs. Do not derive from this type unless you need a Null Object
    /// with extra methods, or are using the class for testing.
    /// </summary>
    public class NullSequence : SequenceOutputBase
    {
        private static NullSequence instance;
        public static NullSequence Instance
        {
            get { return instance ?? (instance = new NullSequence()); }
        }

        protected NullSequence()
        {
        }

        public override OutputStructure AsStructure()
        {
            return NullOutputStructure.Instance;
        }

        public override void AddToStructure(OutputStructure structure, string name)
        {
        }

        public override void AddToSequence(SequenceOutput sequence)
        {
        }
    }
}
