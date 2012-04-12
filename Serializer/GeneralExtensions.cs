using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace json
{
    internal static class GeneralExtensions
    {
        public static string FormatWith(this string formatString, params object[] parameters)
        {
            return String.Format(formatString, parameters);
        }

        public static string Join(this IEnumerable<string> strings, string joinWith)
        {
            return String.Join(joinWith, strings.ToArray());
        }

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        public static string Repeat(this string val, int amount)
        {
            StringBuilder builder = new StringBuilder(val.Length * amount);
            for (int i = 0; i < amount; i++) builder.Append(val);
            return builder.ToString();
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T extraValue)
        {
            foreach (T item in enumerable)
            {
                yield return item;
            }
            yield return extraValue;
        }

        public static IEnumerable<T> ToEnumerable<T>(this T value)
        {
            yield return value;
        }

        public static TOutput IfIs<TInput, TOutput>(this object obj, Func<TInput, TOutput> select) where TInput : class
        {
            TInput input = obj as TInput;
            if (input != null)
            {
                return select(input);
            }
            else
            {
                return default(TOutput);
            }
        }

        public static bool IfIs<TInput>(this object obj, Func<TInput, bool> predicate) where TInput : class
        {
            return obj.IfIs<TInput, bool>(predicate);
        }

        public static void IfIs<TInput>(this object obj, Action<TInput> action) where TInput : class
        {
            TInput input = obj as TInput;
            if (input != null) action(input);
        }

        public static bool None<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

        public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> condition)
        {
            return !collection.Any(condition);
        }

        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue val;
            return dictionary.TryGetValue(key, out val) ? val : default(TValue);
        }

        public static bool IsSingular<T>(this IEnumerable<T> enumerable)
        {
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
                return enumerator.MoveNext() && !enumerator.MoveNext();
        }

        public static T SingleOrDefaultIfMore<T>(this IEnumerable<T> enumerable)
        {
            using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    T single = enumerator.Current;
                    if (!enumerator.MoveNext())
                        return single;
                }
                return default(T);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable)
            {
                action(element);
            }
        }

        public static IEnumerable<Type> GetParameterTypes(this MethodInfo method)
        {
            return method.GetParameters()
                .Select(p => p.ParameterType);
        }
    }
}
