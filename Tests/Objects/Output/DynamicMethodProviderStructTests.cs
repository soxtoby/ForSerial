using System.Reflection;
using ForSerial.Objects;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class DynamicMethodProviderStructTests
    {
        private readonly DynamicMethodProvider sut = new DynamicMethodProvider();

        private readonly PropertyInfo publicValueProperty = typeof(Struct).GetProperty("PublicValueProperty");
        private readonly PropertyInfo publicClassProperty = typeof(Struct).GetProperty("PublicClassProperty");
        private readonly FieldInfo publicValueField = typeof(Struct).GetField("PublicValueField");
        private readonly FieldInfo publicClassField = typeof(Struct).GetField("PublicClassField");
        private readonly PropertyInfo privateValueProperty = typeof(Struct).GetProperty("PrivateValueProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly PropertyInfo privateClassProperty = typeof(Struct).GetProperty("PrivateClassProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly FieldInfo privateValueField = typeof(Struct).GetField("privateValueField", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly FieldInfo privateClassField = typeof(Struct).GetField("privateClassField", BindingFlags.Instance | BindingFlags.NonPublic);

        [Test]
        public void GetPublicValueProperty()
        {
            Struct instance = new Struct { PublicValueProperty = 1 };

            GetMethod result = sut.GetPropertyGetter(publicValueProperty);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPublicClassProperty()
        {
            object expected = new object();
            Struct instance = new Struct { PublicClassProperty = expected };

            GetMethod result = sut.GetPropertyGetter(publicClassProperty);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPublicValueField()
        {
            Struct instance = new Struct { PublicValueField = 1 };

            GetMethod result = sut.GetFieldGetter(publicValueField);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPublicClassField()
        {
            object expected = new object();
            Struct instance = new Struct { PublicClassField = expected };

            GetMethod result = sut.GetFieldGetter(publicClassField);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPrivateValueProperty()
        {
            Struct instance = new Struct(privateValueProperty: 1);

            GetMethod result = sut.GetPropertyGetter(privateValueProperty);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPrivateClassProperty()
        {
            object expected = new object();
            Struct instance = new Struct(privateClassProperty: expected);

            GetMethod result = sut.GetPropertyGetter(privateClassProperty);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPrivateValueField()
        {
            Struct instance = new Struct(privateValueField: 1);

            GetMethod result = sut.GetFieldGetter(privateValueField);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPrivateClassField()
        {
            object expected = new object();
            Struct instance = new Struct(privateClassField: expected);

            GetMethod result = sut.GetFieldGetter(privateClassField);

            Assert.AreSame(expected, result(instance));
        }

        private struct Struct
        {
            public Struct(int privateValueField = 0, object privateClassField = null, int privateValueProperty = 0, object privateClassProperty = null)
                : this()
            {
                this.privateValueField = privateValueField;
                this.privateClassField = privateClassField;
                PrivateValueProperty = privateValueProperty;
                PrivateClassProperty = privateClassProperty;
            }

            private int privateValueField;
            private object privateClassField;
            private int PrivateValueProperty { get; set; }
            private object PrivateClassProperty { get; set; }

            public int GetPrivateValueField() { return privateValueField; }
            public object GetPrivateClassField() { return privateClassField; }
            public int GetPrivateValueProperty() { return PrivateValueProperty; }
            public object GetPrivateClassProperty() { return PrivateClassProperty; }

            public int PublicValueField;
            public object PublicClassField;
            public int PublicValueProperty { get; set; }
            public object PublicClassProperty { get; set; }

        }
    }
}
