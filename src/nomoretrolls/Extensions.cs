using System.Diagnostics;

namespace nomoretrolls
{
    internal static class Extensions
    {
        [DebuggerStepThrough]
        public static int ToReturnCode(this bool value) => value ? 0 : 2;
    }
}
