using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Services.Interfaces;
using TestTask.Models;

namespace TestTask.Services.Tests
{
    [TestClass()]
    public class StatisticsServiceTests
    {
        [TestMethod()]
        public void FillSingleLetterStatsTest_StreamNull_ArgumentNullException()
        {
            IReadOnlyStream stream = null;
            Assert.ThrowsException<ArgumentNullException>(() => StatisticsService.FillSingleLetterStats(stream));
        }

        [TestMethod()]
        public void FillSingleLetterStatsTest_EmptyFile_EmptyList()
        {
            string path = @".\TestFiles\Empty.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = 0;
                var actual = StatisticsService.FillSingleLetterStats(stream).Count;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void FillSingleLetterStatsTest_OneSymbolFile_A_1()
        {
            string path = @".\TestFiles\OneSymbol.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expectedCount = 1;
                var actualCount = StatisticsService.FillSingleLetterStats(stream).Count;
                Assert.AreEqual(expectedCount, actualCount);

                var expectedLetter = new LetterStats { Letter = "A", Count = 1 };
                var actualLetter = StatisticsService.FillSingleLetterStats(stream)[0];
                Assert.AreEqual(expectedLetter, actualLetter);
            }
        }

        [TestMethod()]
        public void FillSingleLetterStatsTest_TestTaskKatren()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = new List<LetterStats>
                {
                    new LetterStats{ Letter = "T", Count = 2 },
                    new LetterStats{ Letter = "e", Count = 2 },
                    new LetterStats{ Letter = "s", Count = 2 },
                    new LetterStats{ Letter = "t", Count = 2 },
                    new LetterStats{ Letter = "a", Count = 2 },
                    new LetterStats{ Letter = "k", Count = 1 },
                    new LetterStats{ Letter = "K", Count = 1 },
                    new LetterStats{ Letter = "r", Count = 1 },
                    new LetterStats{ Letter = "n", Count = 1 },
                }.OrderBy(e => e.Letter).ToList();
                var actual = StatisticsService.FillSingleLetterStats(stream).OrderBy(e => e.Letter).ToList();
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        [TestMethod()]
        public void FillDoubleLetterStatsTest_StreamNull_ArgumentNullException()
        {
            IReadOnlyStream stream = null;
            Assert.ThrowsException<ArgumentNullException>(() => StatisticsService.FillDoubleLetterStats(stream));
        }

        [TestMethod()]
        public void FillDoubleLetterStatsTest_EmptyFile_EmptyList()
        {
            string path = @".\TestFiles\Empty.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = 0;
                var actual = StatisticsService.FillDoubleLetterStats(stream).Count;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void FillDoubleLetterStatsTest_OneSymbolFile_EmptyList()
        {
            string path = @".\TestFiles\OneSymbol.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = 0;
                var actual = StatisticsService.FillDoubleLetterStats(stream).Count;
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void FillDoubleLetterStatsTest_TestTaskKatren()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = new List<LetterStats>
                {
                    new LetterStats{ Letter = "TT", Count = 1 },
                }.OrderBy(e => e.Letter).ToList();
                var actual = StatisticsService.FillDoubleLetterStats(stream).OrderBy(e => e.Letter).ToList();
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        [TestMethod()]
        public void RemoveCharStatsByTypeTest_TestTaskKatren_RemoveVowels()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = new List<LetterStats>
                {
                    new LetterStats{ Letter = "T", Count = 2 },
                    new LetterStats{ Letter = "s", Count = 2 },
                    new LetterStats{ Letter = "t", Count = 2 },
                    new LetterStats{ Letter = "k", Count = 1 },
                    new LetterStats{ Letter = "K", Count = 1 },
                    new LetterStats{ Letter = "r", Count = 1 },
                    new LetterStats{ Letter = "n", Count = 1 },
                }.OrderBy(e => e.Letter).ToList();
                var actual = StatisticsService.FillSingleLetterStats(stream).OrderBy(e => e.Letter).ToList();
                StatisticsService.RemoveCharStatsByType(actual, Enums.CharType.Vowel);
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }
        [TestMethod()]
        public void RemoveCharStatsByTypeTest_TestTaskKatren_RemoveConsonants()
        {
            string path = @".\TestFiles\TestTask Katren.txt";
            using (IReadOnlyStream stream = new ReadOnlyStreamReader(path))
            {
                var expected = new List<LetterStats>
                {
                    new LetterStats{ Letter = "e", Count = 2 },
                    new LetterStats{ Letter = "a", Count = 2 },
                }.OrderBy(e => e.Letter).ToList();
                var actual = StatisticsService.FillSingleLetterStats(stream).OrderBy(e => e.Letter).ToList();
                StatisticsService.RemoveCharStatsByType(actual, Enums.CharType.Consonants);
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }
    }
}