using System.Linq;

namespace TestTask
{
    public static class CharExtensions
    {
        private static readonly char[] vowels = new[] { 'А', 'Е', 'Ё', 'И', 'О', 'У', 'Ы', 'Э', 'Ю', 'Я' };
        public static CharType GetCharType(this char ch)
        {
            if (vowels.Contains(char.ToUpper(ch)))
            {
                return CharType.Vowel;
            }
            return CharType.Consonants;
        }

        public static bool IsLetter(this char ch)
        {
            var workCh = char.ToUpper(ch);
            return workCh >= 'А' && workCh <= 'Я';
        }

        public static bool CompareIgnoreCases(char char1, char char2)
        {
            return char.ToUpper(char1) == char.ToUpper(char2);
        }
    }
}
