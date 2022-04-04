using System.Diagnostics;
using System.Reflection;

namespace nomoretrolls
{
    internal static class Extensions
    {
        [DebuggerStepThrough]
        public static int ToReturnCode(this bool value) => value ? 0 : 2;

        [DebuggerStepThrough]
        public static T ArgNotNull<T>(this T value, string paramName) where T : class
        {
            if (ReferenceEquals(null, value))
            {
                throw new ArgumentNullException(paramName: paramName);
            }
            return value;
        }

        [DebuggerStepThrough]
        public static T InvalidOpArg<T>(this T value, Func<T, bool> predicate, string message)
        {
            predicate.ArgNotNull(nameof(predicate));

            if (ReferenceEquals(null, value) || predicate(value))
            {
                throw new InvalidOperationException(message);
            }

            return value;
        }

        [DebuggerStepThrough]
        public static TResult Pipe<TValue, TResult>(this TValue value, Func<TValue, TResult> selector)
        {
            selector.ArgNotNull(nameof(selector));

            return selector(value);
        }


        [DebuggerStepThrough]
#pragma warning disable CS8601 // Possible null reference assignment.
        public static TResult PipeIfNotNull<TValue, TResult>(this TValue value, Func<TValue, TResult> selector, TResult defaultValue = default)
#pragma warning restore CS8601 // Possible null reference assignment.
                where TValue : class
        {
            selector.ArgNotNull(nameof(selector));

            return value != null
                ? selector(value)
                : defaultValue;
        }

        [DebuggerStepThrough]
        public static string Join(this IEnumerable<string> values, string delimiter) => string.Join(delimiter, values);

        [DebuggerStepThrough]
        public static string Format(this string value, string format) => string.Format(format, value);

        [DebuggerStepThrough]
        public static string GetAttributeValue<T>(this IEnumerable<Attribute> attributes, Func<T, string> selector)
            where T : Attribute
            =>  attributes.OfType<T>().FirstOrDefault().PipeIfNotNull(selector);


        [DebuggerStepThrough]
        public static IEnumerable<(MemberInfo, T)> GetMemberAttributePairs<T>(this Type type)
            where T : Attribute        
            => type.GetMembers().Select(mi => (mi, mi.GetCustomAttributes()
                    .OfType<T>()
                    .FirstOrDefault()))
                    .Where(t => t.Item2 != null);
    }
}
