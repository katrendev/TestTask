using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var inputStream1 = default(IReadOnlyStream);
            var inputStream2 = default(IReadOnlyStream);

            try
            {
                inputStream1 = GetInputStream(args[0]);
                inputStream2 = GetInputStream(args[1]);

                var singleLetterStats = LetterStatsFiller.FillSingleLetterStats(inputStream1).ToList();
                var doubleLetterStats = LetterStatsFiller.FillDoubleLetterStats(inputStream2).ToList();

                RemoveCharStatsByType(singleLetterStats, LetterType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, LetterType.Consonant);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                inputStream1?.Close();
                inputStream2?.Close();
            }

            Console.ReadKey();
        }

        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            {
                throw new FileNotFoundException("File not found", fileFullPath);
            }

            return new ReadOnlyStream(new FileStream(fileFullPath, FileMode.Open, FileAccess.Read));
        }

        public static void RemoveCharStatsByType(List<LetterStats> letters, LetterType letterType)
        {
            var alphabetProvider = new AlphabetProvider();
            letters.RemoveAll(stats => stats.Letter.GetLetterType(alphabetProvider) == letterType);
        }

        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var total = 0;
            var sortedStats = letters.OrderBy(stats => char.ToLower(stats.Letter));

            foreach (var letter in sortedStats)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
                total += letter.Count;
            }
            Console.WriteLine($"Total: {total}");
        }
    }
}