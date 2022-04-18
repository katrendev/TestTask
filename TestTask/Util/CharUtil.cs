using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Util
{
    public static class CharUtil
    {
        public const string CyrillicLettersWithAnalogs = "АВСЕНКМОРТХУавсенкмортху";
        public const string LatinLettersWithAnalogs = "ABCEHKMOPTXYabcehkmoptxy";

        private const string Vowels = "AEIOUYАЕЁИОУЫЭЮЯ";
        private const string Consonants = "BCDFGHJKLMNPQRSTVWXZБВГДЖЗЙКЛМНПРСТФХЦЧШЩЪЬ";

        private static readonly IDictionary<char, char> DicCyrillicToLatinLetters =
            CyrillicLettersWithAnalogs
                .Zip(LatinLettersWithAnalogs, (cyrillic, latin) => new {Cyrillic = cyrillic, Latin = latin})
                .ToDictionary(t => t.Cyrillic, t => t.Latin);

        public static char ToLatinAnalog(this char ch) =>
            DicCyrillicToLatinLetters.TryGetValue(ch, out var res) ? res : ch;

        public static bool Matches(this CharType charType, char ch)
        {
            switch (charType)
            {
                case CharType.Vowel: return Vowels.Contains(ch);
                case CharType.Consonant: return Consonants.Contains(ch);
                default: throw new ArgumentOutOfRangeException(nameof(charType), charType, null);
            }
        }
    }
}