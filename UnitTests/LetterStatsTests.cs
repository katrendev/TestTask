using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class LetterStatsTests
    {
        public void AssertEqual(LetterStats excepted, LetterStats actual)
        {
            Assert.AreEqual(excepted.Count, actual.Count);
            Assert.AreEqual(excepted.Letter, actual.Letter);
        }

        [TestMethod]
        public void EmptyStreamSingleTest()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> singleLetterStats = analyzer.FillSingleLetterStats(inputStream);

                Assert.AreEqual(singleLetterStats.Count, 0);
            }
        }

        [TestMethod]
        public void SmallStreamSingleTest()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("A"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("A", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> singleLetterStats = analyzer.FillSingleLetterStats(inputStream);

                Assert.AreEqual(singleLetterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != singleLetterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], singleLetterStats[i]);
                }
            }
        }

        [TestMethod]
        public void SmallStreamSingleTestWithNoLetters()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("!A!"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("A", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> singleLetterStats = analyzer.FillSingleLetterStats(inputStream);

                Assert.AreEqual(singleLetterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != singleLetterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], singleLetterStats[i]);
                }
            }
        }

        [TestMethod]
        public void BigStreamSingleTest()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("AaBaABC"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("A", 2),
                new LetterStats("a", 2),
                new LetterStats("B", 2),
                new LetterStats("C", 1),
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> singleLetterStats = analyzer.FillSingleLetterStats(inputStream);

                Assert.AreEqual(singleLetterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != singleLetterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], singleLetterStats[i]);
                }
            }
        }

        [TestMethod]
        public void EmptyStreamDoubleTest()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, 0);
            }
        }

        [TestMethod]
        public void SmallStreamDoubleTestWithoutPairs()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("A"));

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, 0);
            }
        }

        [TestMethod]
        public void SmallStreamDoubleTestWithoutPairs2()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Aba"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, 0);
            }
        }

        [TestMethod]
        public void SmallStreamDoubleTestWithPairDif()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Aa"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("AA", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != letterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], letterStats[i]);
                }
            }
        }

        [TestMethod]
        public void SmallStreamDoubleTestWithPair()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("AA"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("AA", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != letterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], letterStats[i]);
                }
            }
        }

        [TestMethod]
        public void BigStreamDoubleTestWithPairs()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("bAacWwwEAAbcB"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("AA", 2),
                new LetterStats("WW", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != letterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], letterStats[i]);
                }
            }
        }

        [TestMethod]
        public void BigStreamDoubleTestWithPairsWithNoLetters()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("!b!!Aac!WwwE!AAbcB!"));
            IList<LetterStats> exceptedLetterStatsList = new List<LetterStats>()
            {
                new LetterStats("AA", 2),
                new LetterStats("WW", 1)
            };

            using (IReadOnlyStream inputStream = new ReadOnlyStream(stream))
            {
                LetterAnalyzer analyzer = new LetterAnalyzer();

                IList<LetterStats> letterStats = analyzer.FillDoubleLetterStats(inputStream);

                Assert.AreEqual(letterStats.Count, exceptedLetterStatsList.Count);

                if (exceptedLetterStatsList.Count != letterStats.Count)
                    return;

                for (int i = 0; i < exceptedLetterStatsList.Count; i++)
                {
                    AssertEqual(exceptedLetterStatsList[i], letterStats[i]);
                }
            }
        }
    }
}
