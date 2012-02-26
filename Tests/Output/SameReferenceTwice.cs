namespace json.Tests
{
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
}