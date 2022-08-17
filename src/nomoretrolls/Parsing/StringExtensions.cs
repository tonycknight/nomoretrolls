using System.Text;

namespace nomoretrolls.Parsing
{
    internal static class StringExtensions
    {
        public static bool IsAllCapitals(this string word) => word.Length > 0 && word.All(char.IsUpper);
                
        public static IEnumerable<string> SplitWords(this string value)
        {
            var sb = new StringBuilder();
            foreach(var c in value)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
                else if(sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb = new StringBuilder();
                }                
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();                
            }
        }

        public static (int, int, int) WordCapitalsSpread(this IEnumerable<string> words)
        {
            int wordCount = 0;
            int capitalsCount = 0;
            int maxWordLength = 0;

            foreach(var word in words)
            {
                wordCount++;
                if (word.IsAllCapitals())
                {
                    capitalsCount++;
                }
                // Stryker disable once all
                if (word.Length >= maxWordLength)
                {
                    maxWordLength = word.Length;
                }
            }
            
            return (wordCount, capitalsCount, maxWordLength);

        }
        
        public static TextCapitalsAnalysis AnalyseCapitals(this string value)
        {
            var letters = value.LetterCount();
            var capitals = value.CapitalCount();

            return new TextCapitalsAnalysis()
            {
                LetterCount = (int)letters,
                CapitalCount = (int)capitals,
                CapitalRatio = letters == 0 ? 0.0 : capitals / letters,
                CapitalGini = value.CapitalGiniImpurity(),
            };
        }

        public static double LetterCount(this string value)
            => value.Count(char.IsLetter);

        public static double CapitalCount(this string value)
            => value.Count(c => char.IsLetter(c) && char.IsUpper(c));

        public static double CapitalGiniImpurity(this string value) 
            => value.Where(char.IsLetter)
                    .Select(char.IsUpper)
                    .GiniImpurity();

        public static string? EmptyWhitespaceToNull(this string value) => string.IsNullOrWhiteSpace(value) ? null : value;

        public static (string?, string?) DeconstructDiscordName(this string userName)
        {
            var idx = userName.IndexOf("#");
            if(idx == -1)
            {
                return (null, null);
            }
            var un = userName.Substring(0, idx).EmptyWhitespaceToNull();
            var discrim = userName.Substring(idx + 1).EmptyWhitespaceToNull();

            if(un == null || discrim == null)
            {
                return (null, null);
            }

            return (un, discrim);
        }
    }
}
