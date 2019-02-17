using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public class LetterStats
    {
        private Dictionary<string, int> letterStorage;
        public int lettersLength { get; set; }
        private string vowel = "уеыаоэяиёюУЕЫАОЭЯИЁЮ";
        

        public LetterStats()
        {
            letterStorage = new Dictionary<string, int>();
        }

        public void AddElement(string letter)
        {
            if (letterStorage.ContainsKey(letter))
            {
                letterStorage[letter]++;
            }
            else
            {
                letterStorage.Add(letter, 1);
            }
            lettersLength += letter.Length;
        }

        private void DeteteElement(string letter)
        {
            lettersLength -= letterStorage[letter] * letter.Length;
            letterStorage.Remove(letter);
        }

        public void RemoveCharStatsByType(CharType charType)
        {
            var keys = letterStorage.Keys.ToList();
            foreach (var key in keys)
            {
                int vowelcount = 0;
                foreach (var letter in key)
                {
                    if (vowel.IndexOf(letter) != -1)
                        vowelcount++;
                }
                if ((charType == CharType.Vowel && vowelcount == key.Length) || (charType == CharType.Consonants && vowelcount == 0))
                {
                    DeteteElement(key);
                }
            }
        }

        public void WriteToConsole()
        {
            var letterslist = letterStorage.Keys.ToList();
            letterslist.Sort();
            foreach (var letter in letterslist)
            {
                Console.WriteLine("{0}:{1}", letter, letterStorage[letter]);
            }
        }
    }
}
