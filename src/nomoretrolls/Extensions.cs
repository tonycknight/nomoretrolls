using System.Diagnostics;
using System.Reflection;

namespace nomoretrolls
{
    internal static class Extensions
    {
        [DebuggerStepThrough]
        public static int ToReturnCode(this bool value) => value ? 0 : 2;

        [DebuggerStepThrough]
        public static string GetAttributeValue<T>(this IEnumerable<Attribute> attributes, Func<T, string> selector)
            where T : Attribute
            =>  attributes.OfType<T>().Select(selector).FirstOrDefault();

        [DebuggerStepThrough]
        public static IEnumerable<(MemberInfo, T)> GetMemberAttributePairs<T>(this Type type)
            where T : Attribute        
            => type.GetMembers().Select(mi => (mi, mi.GetCustomAttributes()
                    .OfType<T>()
                    .FirstOrDefault()))
                    .Where(t => t.Item2 != null);
    }
}
