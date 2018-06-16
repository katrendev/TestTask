using System.Collections.Generic;

namespace TestTask
{
    /// <summary>
    ///    En-ru алфавит
    /// </summary>
    public class MultilingualAlphabetProvider : IAlphabetProvider
    {
        public IEnumerable<char> Alphabet => "abcdefghijklmnopqrstuvwxyz" + "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";

        public IEnumerable<char> Vowels => "aeiou" + "аоиеёэыуюя";

        public IEnumerable<char> Consonants => "bcdfghjklmnpqrstvwxyz" + "бвгджзйклмнпрстфхцчшщ";
    }
}