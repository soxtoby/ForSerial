using System.Reflection;
using json.Objects;
using NUnit.Framework;

namespace json.Tests.Objects
{
    [TestFixture]
    public class DynamicMethodProviderClassTests
    {
        private readonly DynamicMethodProvider sut = new DynamicMethodProvider();

        private readonly PropertyInfo publicValueProperty = typeof(Class).GetProperty("PublicValueProperty");
        private readonly PropertyInfo publicClassProperty = typeof(Class).GetProperty("PublicClassProperty");
        private readonly FieldInfo publicValueField = typeof(Class).GetField("PublicValueField");
        private readonly FieldInfo publicClassField = typeof(Class).GetField("PublicClassField");
        private readonly PropertyInfo privateValueProperty = typeof(Class).GetProperty("PrivateValueProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly PropertyInfo privateClassProperty = typeof(Class).GetProperty("PrivateClassProperty", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly FieldInfo privateValueField = typeof(Class).GetField("privateValueField", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly FieldInfo privateClassField = typeof(Class).GetField("privateClassField", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly MethodInfo publicValueMethod = typeof(Class).GetMethod("PublicValueMethod", new[] { typeof(int) });
        private readonly MethodInfo privateValueMethod = typeof(Class).GetMethod("PrivateValueMethod", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(int) }, new ParameterModifier[] { });
        private readonly MethodInfo publicClassMethod = typeof(Class).GetMethod("PublicClassMethod", new[] { typeof(object) });
        private readonly MethodInfo privateClassMethod = typeof(Class).GetMethod("PrivateClassMethod", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(object) }, new ParameterModifier[] { });

        [Test]
        public void GetPublicValueProperty()
        {
            Class instance = new Class { PublicValueProperty = 1 };

            GetMethod result = sut.GetPropertyGetter(publicValueProperty);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPublicClassProperty()
        {
            object expected = new object();
            Class instance = new Class { PublicClassProperty = expected };

            GetMethod result = sut.GetPropertyGetter(publicClassProperty);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPublicValueField()
        {
            Class instance = new Class { PublicValueField = 1 };

            GetMethod result = sut.GetFieldGetter(publicValueField);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPublicClassField()
        {
            object expected = new object();
            Class instance = new Class { PublicClassField = expected };

            GetMethod result = sut.GetFieldGetter(publicClassField);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPrivateValueProperty()
        {
            Class instance = new Class(privateValueProperty: 1);

            GetMethod result = sut.GetPropertyGetter(privateValueProperty);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPrivateClassProperty()
        {
            object expected = new object();
            Class instance = new Class(privateClassProperty: expected);

            GetMethod result = sut.GetPropertyGetter(privateClassProperty);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void GetPrivateValueField()
        {
            Class instance = new Class(privateValueField: 1);

            GetMethod result = sut.GetFieldGetter(privateValueField);

            Assert.AreEqual(1, result(instance));
        }

        [Test]
        public void GetPrivateClassField()
        {
            object expected = new object();
            Class instance = new Class(privateClassField: expected);

            GetMethod result = sut.GetFieldGetter(privateClassField);

            Assert.AreSame(expected, result(instance));
        }

        [Test]
        public void SetPublicValueProperty()
        {
            Class instance = new Class();

            SetMethod result = sut.GetPropertySetter(publicValueProperty);

            result(instance, 1);
            Assert.AreEqual(1, instance.PublicValueProperty);
        }

        [Test]
        public void SetPublicClassProperty()
        {
            Class instance = new Class();
            object expected = new object();

            SetMethod result = sut.GetPropertySetter(publicClassProperty);

            result(instance, expected);
            Assert.AreSame(expected, instance.PublicClassProperty);
        }

        [Test]
        public void SetPublicValueField()
        {
            Class instance = new Class();

            SetMethod result = sut.GetFieldSetter(publicValueField);

            result(instance, 1);
            Assert.AreEqual(1, instance.PublicValueField);
        }

        [Test]
        public void SetPublicClassField()
        {
            Class instance = new Class();
            object expected = new object();

            SetMethod result = sut.GetFieldSetter(publicClassField);

            result(instance, expected);
            Assert.AreSame(expected, instance.PublicClassField);
        }

        [Test]
        public void SetPrivateValueProperty()
        {
            Class instance = new Class();

            SetMethod result = sut.GetPropertySetter(privateValueProperty);

            result(instance, 1);
            Assert.AreEqual(1, instance.GetPrivateValueProperty());
        }

        [Test]
        public void SetPrivateClassProperty()
        {
            Class instance = new Class();
            object expected = new object();

            SetMethod result = sut.GetPropertySetter(privateClassProperty);

            result(instance, expected);
            Assert.AreSame(expected, instance.GetPrivateClassProperty());
        }

        [Test]
        public void SetPrivateValueField()
        {
            Class instance = new Class();

            SetMethod result = sut.GetFieldSetter(privateValueField);

            result(instance, 1);
            Assert.AreEqual(1, instance.GetPrivateValueField());
        }

        [Test]
        public void SetPrivateClassField()
        {
            Class instance = new Class();
            object expected = new object();

            SetMethod result = sut.GetFieldSetter(privateClassField);

            result(instance, expected);
            Assert.AreSame(expected, instance.GetPrivateClassField());
        }

        [Test]
        public void CallPublicValueMethod()
        {
            Class instance = new Class();

            ActionMethod result = sut.GetAction(publicValueMethod);

            result(instance, new object[] { 1 });
            Assert.AreEqual(1, instance.GetPrivateValueField());
        }

        [Test]
        public void CallPrivateValueMethod()
        {
            Class instance = new Class();

            ActionMethod result = sut.GetAction(privateValueMethod);

            result(instance, new object[] { 1 });
            Assert.AreEqual(1, instance.GetPrivateValueField());
        }

        [Test]
        public void CallPublicClassMethod()
        {
            Class instance = new Class();
            object expected = new object();

            ActionMethod result = sut.GetAction(publicClassMethod);

            result(instance, new[] { expected });
            Assert.AreSame(expected, instance.GetPrivateClassField());
        }

        [Test]
        public void CallPrivateClassMethod()
        {
            Class instance = new Class();
            object expected = new object();

            ActionMethod result = sut.GetAction(privateClassMethod);

            result(instance, new[] { expected });
            Assert.AreSame(expected, instance.GetPrivateClassField());
        }

        private class Class
        {
            public Class(int privateValueField = 0, object privateClassField = null, int privateValueProperty = 0, object privateClassProperty = null)
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

            public void PublicValueMethod(int value)
            {
                privateValueField = value;
            }

            private void PrivateValueMethod(int value)
            {
                privateValueField = value;
            }

            public void PublicClassMethod(object obj)
            {
                privateClassField = obj;
            }

            private void PrivateClassMethod(object obj)
            {
                privateClassField = obj;
            }

            public int PublicValueField;
            public object PublicClassField;
            public int PublicValueProperty { get; set; }
            public object PublicClassProperty { get; set; }
        }
    }
}
