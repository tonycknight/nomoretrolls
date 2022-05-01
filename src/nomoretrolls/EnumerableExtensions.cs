namespace nomoretrolls
{
    internal static class EnumerableExtensions
    {
        public static double Entropy<T>(this IEnumerable<T> values)
            where T : IEquatable<T>
        {
            double result = 0;
            var uniques = values.GroupBy(x => x)
                                .Select(grp => grp.Count());
            var count = (double)uniques.Sum(grp => grp);

            foreach (var grp in uniques)
            {
                var p = (double)grp / count;
                result = result - (p * System.Math.Log2(p));
            }

            return result;
        }

        public static double GiniImpurity<T>(this IEnumerable<T> values)
            where T : IEquatable<T>
        {
            double result = 0;
            var uniques = values.GroupBy(x => x)
                                .Select(grp => grp.Count())
                                .ToList();
            var count = (double)uniques.Sum(grp => grp);

            for (var j = 0; j < uniques.Count; j++)
            {
                var p1 = (double)uniques[j] / count;

                for (var k = 0; k < uniques.Count; k++)
                {
                    if (j != k)
                    {
                        var p2 = (double)uniques[k] / count;
                        result += p1 * p2;
                    }
                }
            }
            return result;
        }
    }
}
