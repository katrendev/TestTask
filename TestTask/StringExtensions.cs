using System.Linq;

namespace TestTask
{
    internal static class StringExtensions
    {
        public static bool ContainsOnlyConsonants(this string s)
        {
            return s.ToCharArray().All(x => GetCharType(x) == CharType.Consonants);
        }
        public static bool ContainsOnlyVowels(this string s)
        {
            return s.ToCharArray().All(x => GetCharType(x) == CharType.Vowel);
        }

        private static CharType GetCharType(char c)
        {
            var vowels = "уУеЕыЫаАоОэЭяЯиИюЮ" + "eEyYuUiIoOaA";
            if (vowels.ToCharArray().Contains(c))
            {
                return CharType.Vowel;
            }
            else
            {
                return CharType.Consonants;
            }
        }
    }
}
