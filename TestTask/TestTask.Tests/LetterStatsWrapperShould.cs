using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TestTask.Tests
{
    public class LetterStatsWrapperShould
    {
        private IReadOnlyStream GenerateStreamFromString(string input)
        {
            return new ReadOnlyStream(new MemoryStream(Encoding.UTF8.GetBytes(input ?? "")));
        }
        
        /// <summary>
        ///     Тест на простой подсчет каждой буквы.
        /// </summary>
        /// <param name="input">Входная строка для инициализации потока</param>
        [TestCase("Lorem Ipsum - simply dummy text")]
        [TestCase("это текст-рыба")]
        [TestCase("ը տպագրության և տպագրական")]
        [TestCase("也称乱数假文或者哑元文本")]
        public void FillSingleLetterStats_Counting(string input)
        {
            var inputStream = GenerateStreamFromString(input);

            var singleLetterStats = LetterStatsWrapper.FillSingleLetterStats(inputStream);
            foreach (var letterStat in singleLetterStats)
            {
                var expected = input.Count(c => c == letterStat.Letter);
                var actual = letterStat.Count;
                Assert.AreEqual(expected, actual);
            }
            
            inputStream.Close();
        }
        
        [TestCase("Lorem Ipsum - simply dummy text", LetterType.Vowel, "Lrm psm - smply dmmy txt")]
        [TestCase("Lorem Ipsum - это текст-рыба", LetterType.Consonant, "oe Iu - эо е-ыа")]
        [TestCase("Язь", LetterType.Aphonic, "Яз")]
        public void LetterStatsWrapper_RemoveCharStatsByType(string input, LetterType toRemove, string expected)
        {
            var inputStream = GenerateStreamFromString(input);
            var expectedStream = GenerateStreamFromString(expected);

            var actualStats = LetterStatsWrapper.FillSingleLetterStats(inputStream).ToList();
            var expectedStats = LetterStatsWrapper.FillSingleLetterStats(expectedStream).ToList();

            LetterStatsWrapper.RemoveCharStatsByType(actualStats, toRemove);

            Assert.AreEqual(expectedStats.OrderBy(stats => stats.Letter).Select(stats => stats.Letter), 
                actualStats.OrderBy(stats => stats.Letter).Select(stats => stats.Letter));
        }
    }
}