using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Пожалуйста, укажите два пути к файлам.");
                return;
            }

            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            var singleLetterStats = FillSingleLetterStats(inputStream1);
            var doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Нажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new FileReader(fileFullPath);
        }

        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            var stats = new Dictionary<char, LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!char.IsLetter(c))
                    continue;

                if (!stats.ContainsKey(c))
                    stats[c] = new LetterStats { Letter = c.ToString(), Count = 0 };

                var current = stats[c];
                IncStatistic(ref current);
                stats[c] = current;

            }

            return new List<LetterStats>(stats.Values);
        }

        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            var stats = new Dictionary<string, LetterStats>();

            stream.ResetPositionToStart();
            char? prevChar = null;

            while (!stream.IsEof)
            {
                char current = stream.ReadNextChar();

                if (!char.IsLetter(current))
                {
                    prevChar = null;
                    continue;
                }

                if (prevChar.HasValue && char.ToLower(prevChar.Value) == char.ToLower(current))
{
    string key = char.ToUpper(current).ToString() + char.ToUpper(current).ToString();

    if (!stats.ContainsKey(key))
        stats[key] = new LetterStats { Letter = key, Count = 0 };

    var statEntry = stats[key];
    IncStatistic(ref statEntry);
    stats[key] = statEntry;
}


                prevChar = current;
            }

            return new List<LetterStats>(stats.Values);
        }

        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            string vowels = "аеёиоуыэюяАЕЁИОУЫЭЮЯ";
            string consonants = "бвгджзйклмнпрстфхцчшщБВГДЖЗЙКЛМНПРСТФХЦЧШЩ";

            for (int i = letters.Count - 1; i >= 0; i--)
            {
                string s = letters[i].Letter;

                if (s.Length == 1)
                {
                    if (charType == CharType.Vowel && vowels.Contains(s[0]))
                        letters.RemoveAt(i);
                    else if (charType == CharType.Consonants && consonants.Contains(s[0]))
                        letters.RemoveAt(i);
                }
                else if (s.Length == 2)
                {
                    if (charType == CharType.Vowel && vowels.Contains(s[0]) && vowels.Contains(s[1]))
                        letters.RemoveAt(i);
                    else if (charType == CharType.Consonants && consonants.Contains(s[0]) && consonants.Contains(s[1]))
                        letters.RemoveAt(i);
                }
            }
        }

        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            int total = 0;
            foreach (var stat in letters.OrderBy(l => l.Letter))
            {
                Console.WriteLine($"{stat.Letter} : {stat.Count}");
                total += stat.Count;
            }

            Console.WriteLine($"ИТОГО: {total}");
        }

        private static void IncStatistic(ref LetterStats stat)
        {
            stat.Count++;
        }
    }
}
