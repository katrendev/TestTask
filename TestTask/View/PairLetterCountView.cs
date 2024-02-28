﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask.View
{
    public class PairLetterCountView : ILetterCountView
    {
        public void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var letterStatsEnumerable = letters as LetterStats[] ?? letters.ToArray();
            var orderedLetters = letterStatsEnumerable.OrderBy(x => x.Letter);

            Console.WriteLine("Статистика вхождения парных букв:");

            int count = 0;

            foreach (var letterStats in orderedLetters)
            {
                Console.WriteLine("{" + letterStats.Letter + "} " + ":" + " {" + letterStats.Count + "}");
                count += letterStats.Count;
            }

            Console.WriteLine("Итого " + count);
        }
    }
}
