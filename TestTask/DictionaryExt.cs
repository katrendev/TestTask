using System;
using System.Collections.Generic;

namespace TestTask
{
    public static class DictionaryExt
    {
        public static void AddToDictionary(this Dictionary<string, LetterStats> dict, string letter)
        {
            if (dict.ContainsKey(letter))
            {
                dict[letter] = dict[letter].IncreaseCount();
                return;
            }

            dict[letter] = new LetterStats(letter, 1);
        }
    }
}