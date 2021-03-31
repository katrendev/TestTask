using System;
using System.Linq;

namespace TestTask
{
    public static class CharExtension
    {
        private static readonly char[] Vowels = { 'А', 'Е', 'Ё', 'У', 'Ы', 'Я', 'И', 'О', 'Ю', 'Э',
                                                  'A', 'E', 'I', 'O', 'U', 'Y' };

        private static readonly char[] Consonants = { 'Б', 'В', 'Г', 'Д', 'Ж', 'З', 'Й', 'К', 'Л', 'М', 'Н', 'П', 'Р', 'С', 'Т', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ',
                                                      'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z'};

        /// <summary>
        /// Определить тип буквы
        /// </summary>
        /// <param name="c">Символ для определения</param>
        /// <returns></returns>
        public static CharType GetCharType(this char c)
        {
            if (Vowels.Contains(Char.ToUpper(c)))
            {
                return CharType.Vowel;
            }
            else if (Consonants.Contains(Char.ToUpper(c)))
            {
                return CharType.Consonants;
            }
            else
            {
                return CharType.Unknown;
            }
        }
    }
}
