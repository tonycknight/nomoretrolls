using System.Diagnostics;

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
        public static string Join(this IEnumerable<string> values, string delimiter) => string.Join(delimiter, values);
    }
}
