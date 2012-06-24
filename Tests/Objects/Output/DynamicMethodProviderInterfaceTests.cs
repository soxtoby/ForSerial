using System.Reflection;
using EasyAssertions;
using ForSerial.Objects;
using NUnit.Framework;

namespace ForSerial.Tests.Objects
{
    [TestFixture]
    public class DynamicMethodProviderInterfaceTests
    {
        private readonly DynamicMethodProvider sut = new DynamicMethodProvider();

        private readonly PropertyInfo property = typeof(Interface).GetProperty("Property");
        private readonly MethodInfo function = typeof(Interface).GetMethod("Function");
        private readonly MethodInfo action = typeof(Interface).GetMethod("Action");

        [Test]
        public void GetPropertyGetter_FromInterface_ReturnsNull()
        {
            sut.GetPropertyGetter(property)
                .ShouldBeNull();
        }

        [Test]
        public void GetPropertySetter_FromInterface_ReturnsNull()
        {
            sut.GetPropertySetter(property)
                .ShouldBeNull();
        }

        [Test]
        public void GetAction_FromInterface_ReturnsNull()
        {
            sut.GetAction(action)
                .ShouldBeNull();
        }

        private interface Interface
        {
            int Property { get; set; }
            void Action();
        }
    }
}