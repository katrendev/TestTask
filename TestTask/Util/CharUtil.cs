using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.Util
{
    public static class CharUtil
    {
        public const string CyrillicLettersWithAnalogs = "АВСЕНКМОРТХУавсенкмортху";
        public const string LatinLettersWithAnalogs = "ABCEHKMOPTXYabcehkmoptxy";

        private static readonly ISet<char> Vowels = "AEIOUYАЕЁИОУЫЭЮЯ".ToHashSet();
        private static readonly ISet<char> Consonants = "BCDFGHJKLMNPQRSTVWXZБВГДЖЗЙКЛМНПРСТФХЦЧШЩЪЬ".ToHashSet();

        private static readonly IDictionary<char, char> DicCyrillicToLatinLetters =
            CyrillicLettersWithAnalogs
                .Zip(LatinLettersWithAnalogs, (cyrillic, latin) => new {Cyrillic = cyrillic, Latin = latin})
                .ToDictionary(t => t.Cyrillic, t => t.Latin);

        public static char ToLatinAnalog(this char ch) =>
            DicCyrillicToLatinLetters.TryGetValue(ch, out var res) ? res : ch;

        public static bool Matches(this CharType charType, char ch)
        {
            var upperChar = char.ToUpperInvariant(ch);
            switch (charType)
            {
                case CharType.Vowel: return Vowels.Contains(upperChar);
                case CharType.Consonant: return Consonants.Contains(upperChar);
                default: throw new ArgumentOutOfRangeException(nameof(charType), charType, null);
            }
        }
    }
}