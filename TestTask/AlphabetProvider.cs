using System.Collections.Generic;

namespace TestTask
{
    public class AlphabetProvider : IAlphabetProvider
    {
        public IEnumerable<char> Alphabet => "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщьыъэюя";

        public IEnumerable<char> Vowels => "aeiouаоиеёэыуюя";

        public IEnumerable<char> Consonants => "bcdfghjklmnpqrstvwxyzбвгджзйклмнпрстфхцчшщ";
    }
}