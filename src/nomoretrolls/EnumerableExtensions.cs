namespace nomoretrolls
{
    internal static class EnumerableExtensions
    {
        public static TValue? TryGet<TKey, TValue>(this Dictionary<TKey, TValue> values, TKey key)
            where TKey : class
            where TValue : class
        {
            values.ArgNotNull(nameof(values));
            key.ArgNotNull(nameof(key));

            if(values.TryGetValue(key, out var value))
            {
                return value;
            }
            return default(TValue);
        }
    }
}
