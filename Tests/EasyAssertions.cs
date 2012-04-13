using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ForSerial.Objects;

namespace ForSerial.Tests
{
    public static class EasyAssertions
    {
        public static ActualValue<T> ShouldBe<T>(this T actual, T expected, string message = null)
        {
            AssertEqual(actual, expected, message);
            return new ActualValue<T>(actual);
        }

        public static void ShouldBe(this object actual, string expected, string message = null)
        {
            AssertEqual(Convert.ToString(actual), expected, message);
        }

        public static ActualValue<string> ShouldContain(this string actual, string expectedSubstring, string message = null)
        {
            if (!actual.Contains(expectedSubstring))
                throw new EasyAssertionException("\r\nExpected to contain: {0}\r\nActual: {1}\r\n{2}".FormatWith(expectedSubstring, actual, message ?? string.Empty));
            return new ActualValue<string>(actual);
        }

        public static ActualValue<string> ShouldEndWith(this string actual, string expectedEnd, string message = null)
        {
            if (!actual.EndsWith(expectedEnd))
                throw new EasyAssertionException("\r\nExpected to end with: {0}\r\nActual: {1}\r\n{2}".FormatWith(expectedEnd, actual, message ?? string.Empty));
            return new ActualValue<string>(actual);
        }

        public static void ShouldMatch<TExpected>(this IEnumerable actual, IEnumerable<TExpected> expected, string message = null)
        {
            List<object> actuals = actual.Cast<object>().ToList();
            List<TExpected> castActuals = new List<TExpected>();
            Type expectedType = typeof(TExpected);
            foreach (object item in actuals)
            {
                TExpected castItem;
                if (!TryConvert(item, out castItem))
                    throw new EasyAssertionException(
                        BuildCollectionAssertionExceptionMessage(
                            "Expected items to be {0}, but item at index {1} was a {2}.".FormatWith(expectedType.FullName, castActuals.Count, GetTypeName(item)),
                            actuals, expected.Cast<object>(), message));

                castActuals.Add(castItem);
            }

            castActuals.ShouldMatch(expected);
        }

        private static bool TryConvert<T>(object item, out T convertedItem)
        {
            try
            {
                convertedItem = (T)Convert.ChangeType(item, typeof(T));
                return true;
            }
            catch (InvalidCastException)
            {
                convertedItem = default(T);
                return false;
            }
        }

        public static void ShouldBeSameAs<T>(this T actual, T expected, string message = null)
        {
            if (!ReferenceEquals(actual, expected))
                throw new EasyAssertionException("\r\nExpected this: {0}\r\nActual:        {1}\r\n{2}".FormatWith(ConvertToString(expected), ConvertToString(actual), message));
        }

        public static ActualValue<T> ShouldNotBeSameAs<T>(this T actual, T expected, string message = null)
        {
            if (ReferenceEquals(actual, expected))
                throw new EasyAssertionException("Expected anything but {0}.\r\n{1}".FormatWith(expected, message));
            return new ActualValue<T>(actual);
        }

        public static void ShouldMatch<TActual, TItem>(this TActual actual, IEnumerable<TItem> expected, string message = null) where TActual : IEnumerable<TItem>
        {
            List<object> actuals = actual.Cast<object>().ToList();
            List<object> expecteds = expected.Cast<object>().ToList();

            if (actuals.Count != expecteds.Count)
                throw new EasyAssertionException(
                    BuildCollectionAssertionExceptionMessage(
                        "Expected {0} items, but there were {1}.".FormatWith(expecteds.Count, actuals.Count), actuals, expecteds, message));

            for (int i = 0; i < expecteds.Count; i++)
            {
                if (!AreEqual(actuals[i], expecteds[i]))
                    throw new EasyAssertionException(
                        BuildCollectionAssertionExceptionMessage(
                            "Difference at index {0}.\r\nExpected item: {1}\r\nActual item:   {2}".FormatWith(i, expecteds[i], actuals[i]), actuals, expecteds, message));
            }
        }

        private static string BuildCollectionAssertionExceptionMessage(string failedAssertionMessage, IEnumerable<object> actuals, IEnumerable<object> expecteds, string userMessage)
        {
            return "{0}\r\nExpected: {1}\r\nActual:   {2}\r\n{3}".FormatWith(
                failedAssertionMessage,
                ConvertToString(expecteds),
                ConvertToString(actuals),
                userMessage);
        }

        public static void ShouldBeNull<T>(this T actual, string message = null) where T : class
        {
            if (actual != null)
                throw new EasyAssertionException("Expected null, but was {0}.\r\n{1}".FormatWith(actual, message));
        }

        public static ActualValue<T> ShouldNotBeNull<T>(this T actual, string message = null) where T : class
        {
            if (actual == null)
                throw new EasyAssertionException("Shouldn't have been null.\r\n{0}".FormatWith(message ?? string.Empty));

            return new ActualValue<T>(actual);
        }

        public static ActualValue<TExpected> ShouldBe<TExpected>(this object actual, string message = null)
        {
            if (!(actual is TExpected))
                throw new EasyAssertionException("\r\nExpected {0}, but was {1}.\r\n{2}".FormatWith(typeof(TExpected).FullName, GetTypeName(actual), message));

            return new ActualValue<TExpected>((TExpected)actual);
        }

        private static string GetTypeName(object obj)
        {
            return obj == null
                ? "null"
                : obj.GetType().FullName;
        }

        public static ActualValue<TActual> And<TActual>(this ActualValue<TActual> actual, Action<TActual> assert)
        {
            assert(actual.And);
            return actual;
        }

        public static void ShouldBeEmpty(this IEnumerable enumerable, string message = null)
        {
            List<object> items = enumerable.Cast<object>().ToList();
            if (items.Any())
                throw new EasyAssertionException("Should be empty, but has {0} items:\r\n{1}\r\n{2}".FormatWith(items.Count, items.Select(Convert.ToString).Join(", "), message));
        }

        public static void ItemsSatisfy<T>(this IEnumerable<T> actual, params Action<T>[] asserts)
        {
            List<T> actuals = actual.ToList();
            if (actuals.Count != asserts.Length)
                throw new EasyAssertionException("Expected {0} items, but there were {1}.\r\nActual: {2}".FormatWith(asserts.Length, actuals.Count, ConvertToString(actuals)));

            for (int i = 0; i < actuals.Count; i++)
                asserts[i](actuals[i]);
        }

        private static void AssertEqual(object actual, object expected, string message)
        {
            if (!AreEqual(actual, expected))
                throw new EasyAssertionException("\r\nExpected: {0}\r\nActual:   {1}\r\n{2}".FormatWith(ConvertToString(expected), ConvertToString(actual), message));
        }

        private static bool AreEqual(object actual, object expected)
        {
            if (actual == null && expected == null)
                return true;

            if (actual == null || expected == null)
                return false;

            TypeCode actualTypeCode = Type.GetTypeCode(actual.GetType());
            TypeCodeType actualTypeCodeType = actualTypeCode.GetTypeCodeType();
            TypeCode expectedTypeCode = Type.GetTypeCode(expected.GetType());
            TypeCodeType expectedTypeCodeType = expectedTypeCode.GetTypeCodeType();

            if (actualTypeCodeType != expectedTypeCodeType)
                return false;

            if (actualTypeCode != expectedTypeCode && actualTypeCodeType == TypeCodeType.Number)
                actual = Convert.ChangeType(actual, expectedTypeCode);

            return Equals(actual, expected);
        }

        private static string ConvertToString(object obj)
        {
            string str = Convert.ToString(obj);
            return str == string.Empty
                ? "<String.Empty>"
                : str;
        }

        private static string ConvertToString(IEnumerable list)
        {
            return "[{0}]".FormatWith(list.Cast<object>().Select(Convert.ToString).Join(", "));
        }
    }

    public static class Function
    {
        public static AssertionFunction Call(Expression<Action> functionCall)
        {
            return new AssertionFunction(functionCall);
        }
    }

    public class ActualValue<T>
    {
        public T And { get; private set; }

        public ActualValue(T actual)
        {
            And = actual;
        }
    }

    public class AssertionFunction
    {
        private readonly Expression<Action> functionCall;

        public AssertionFunction(Expression<Action> functionCall)
        {
            this.functionCall = functionCall;
        }

        public ActualValue<T> ShouldThrow<T>() where T : Exception
        {
            try
            {
                functionCall.Compile()();
            }
            catch (T e)
            {
                return new ActualValue<T>(e);
            }
            catch (Exception e)
            {
                throw new EasyAssertionException(functionCall.Body + " should have thrown " + typeof(T).Name + ", but instead threw " + e.GetType().Name);
            }

            throw new EasyAssertionException(functionCall.Body + " should have thrown " + typeof(T).Name + ", but didn't throw at all.");
        }
    }

    public class EasyAssertionException : Exception
    {
        public EasyAssertionException(string message) : base(message) { }
    }
}
