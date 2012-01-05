using System;
using NUnit.Framework;

namespace json.Objects
{
    [TestFixture]
    public class TypedObjectConstructorOnlyObjectTests
    {
        [Test]
        public void SingleMatchingParameter()
        {
            var value = Construct<SingleMatchingParamStruct>(parseObject =>
                parseObject.AddProperty("Value", new TypedPrimitiveValue(5)));
            Assert.AreEqual(5, value.Value);
        }

        [Test]
        public void MatchDifferentNumberTypes()
        {
            var value = Construct<SingleMatchingParamStruct>(parseObject =>
                parseObject.AddProperty("Value", new TypedPrimitiveValue(5d)));
            Assert.AreEqual(5, value.Value);
        }

        [Test]
        [ExpectedException(typeof(ConstructorOnlyObject.NoMatchingConstructor))]
        public void SingleParameterNameMismatch()
        {
            Construct<SingleMatchingParamStruct>(parseObject =>
                parseObject.AddProperty("foo", new TypedPrimitiveValue(5)));
        }

        [Test]
        [ExpectedException(typeof(ConstructorOnlyObject.NoMatchingConstructor))]
        public void SingleParameterTypeMismatch()
        {
            Construct<SingleMatchingParamStruct>(parseObject =>
                parseObject.AddProperty("Value", new TypedPrimitiveValue("foo")));
        }

        private struct SingleMatchingParamStruct
        {
            public SingleMatchingParamStruct(int value)
                : this()
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        [Test]
        public void TwoMatchingParameter()
        {
            var value = Construct<TwoMatchingParameterStruct>(parseObject =>
                {
                    parseObject.AddProperty("IntValue", new TypedPrimitiveValue(4));
                    parseObject.AddProperty("StringValue", new TypedPrimitiveValue("foo"));
                });

            Assert.AreEqual(4, value.IntValue);
            Assert.AreEqual("foo", value.StringValue);
        }

        private struct TwoMatchingParameterStruct
        {
            public TwoMatchingParameterStruct(string stringValue, int intValue)
                : this()
            {
                IntValue = intValue;
                StringValue = stringValue;
            }

            public int IntValue { get; private set; }
            public string StringValue { get; private set; }
        }

        [Test]
        public void SingleMatchingGenericParameter()
        {
            var value = Construct<SingleGenericParamStruct<int>>(parseObject =>
                parseObject.AddProperty("Value", new TypedPrimitiveValue(5)));

            Assert.AreEqual(5, value.Value);
        }

        [Test]
        [ExpectedException(typeof(ConstructorOnlyObject.NoMatchingConstructor))]
        public void SingleGenericParameterTypeMismatch()
        {
            var value = Construct<SingleGenericParamStruct<int>>(parseObject =>
                parseObject.AddProperty("Value", new TypedPrimitiveValue("foo")));

            Assert.AreEqual("foo", value.Value);
        }

        private struct SingleGenericParamStruct<T>
        {
            public SingleGenericParamStruct(T value)
                : this()
            {
                Value = value;
            }

            public T Value { get; private set; }
        }

        [Test]
        public void SingleBaseClassParameter()
        {
            var value = Construct<SingleBaseClassParamStruct>(parseObject =>
                parseObject.AddProperty("property", new TypedPrimitiveValue(new SubClass { BaseProperty = 5 })));

            Assert.AreEqual(5, value.Property.BaseProperty);
        }

        private struct SingleBaseClassParamStruct
        {
            public SingleBaseClassParamStruct(BaseClass property)
                : this()
            {
                Property = property;
            }

            public BaseClass Property { get; private set; }
        }

        private abstract class BaseClass
        {
            public abstract int BaseProperty { get; set; }
        }

        private class SubClass : BaseClass
        {
            public override int BaseProperty { get; set; }
        }

        private static T Construct<T>(Action<ConstructorOnlyObject> populate)
        {
            var parseObject = new ConstructorOnlyObject(CurrentTypeHandler.GetTypeDefinition(typeof(T)));
            populate(parseObject);
            object obj = parseObject.Object;
            Assert.IsInstanceOf<T>(obj);
            return (T)obj;
        }
    }
}