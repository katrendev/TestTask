using NUnit.Framework;
using TestTask;
using System.Collections.Generic;

namespace TestTaskTest
{

    /// <summary>
    /// Тесты для проверки алгоритмов анализа файлов
    /// </summary>
    public class LetterAnalysisFacadeTests
    {
        private LetterAnalysisFacade _facade = new LetterAnalysisFacade();

        [Test]
        public void FillSingleLetterStats_1_IgnoreNonLetters()
        {
            // ARRANGE
            char[] testData = "1".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillSingleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 0);
        }

        [Test]
        public void FillSingleLetterStats_1Letter_Have1Value()
        {
            // ARRANGE
            char[] testData = "A".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillSingleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 1);
        }

        [Test]
        public void FillSingleLetterStats_1LetterA_ResultCount1()
        {
            // ARRANGE
            char[] testData = "A".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillSingleLetterStats(fake);

            // ASERT
            Assert.That(result[0].Count == 1);
        }

        [Test]
        public void FillSingleLetterStats_aabbb_returns2a3b()
        {
            // ARRANGE
            char[] testData = "aabbb".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillSingleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 2);
            LetterStats firstElement = result[0];

            if (firstElement.Letter == "a")
            {
                Assert.That(firstElement.Count == 2);
            }
            else
            {
                Assert.That(firstElement.Letter == "b");
                Assert.That(firstElement.Count == 3);
            }

            LetterStats secondElement = result[1];
            if (firstElement.Letter == "a")
            {
                Assert.That(secondElement.Letter == "b");
                Assert.That(secondElement.Count == 3);
            } 
            else
            {
                Assert.That(secondElement.Letter == "a");
                Assert.That(secondElement.Count == 2);
            }
        }

        [Test]
        public void FillDoubleLetterStats_1_IgnoreNonLetters()
        {
            // ARRANGE
            char[] testData = "1".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillDoubleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 0);
        }

        [Test]
        public void FillDoubleLetterStats_2DifferentLetters_NotCount()
        {
            // ARRANGE
            char[] testData = "AB".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillDoubleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 0);
        }

        [Test]
        public void FillDoubleLetterStats_2SameLetterA_ResultCount1()
        {
            // ARRANGE
            char[] testData = "AA".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillDoubleLetterStats(fake);

            // ASERT
            Assert.That(result[0].Count == 1);
        }

        [Test]
        public void FillDoubleLetterStats_aabbb_returns1AA2BB()
        {
            // ARRANGE
            char[] testData = "aabbb".ToCharArray();

            IReadOnlyStream fake = new ArrayReadOnlyStream(testData);

            // ACT
            IList<LetterStats> result = _facade.FillDoubleLetterStats(fake);

            // ASERT
            Assert.That(result.Count == 2);
            LetterStats firstElement = result[0];

            if (firstElement.Letter == "AA")
            {
                Assert.That(firstElement.Count == 1);
            } 
            else
            {
                Assert.That(firstElement.Letter == "BB");
                Assert.That(firstElement.Count == 2);
            }

            LetterStats secondElement = result[1];
            if (firstElement.Letter == "AA")
            {
                Assert.That(secondElement.Letter == "BB");
                Assert.That(secondElement.Count == 2);
            } 
            else
            {
                Assert.That(secondElement.Letter == "AA");
                Assert.That(secondElement.Count == 1);
            }
        }

        [Test]
        public void RemoveCharStatsByType_a1b1CharTypeVowel_b1Returns()
        {
            // ARRANGE
            IList<LetterStats> stats = new List<LetterStats>
            {
                new LetterStats 
                {
                    Letter = "a",
                    Count = 1
                },
                new LetterStats
                {
                    Letter = "b",
                    Count = 1
                }
            };

            // ACT
            IList<LetterStats> result = _facade.RemoveCharStatsByType(stats, CharType.Vowel);
            
            // ASSERT
            Assert.That(result.Count == 1);
            Assert.That(result[0].Letter == "b");
            Assert.That(result[0].Count == 1);
        }

        [Test]
        public void RemoveCharStatsByType_a1b1CharTypeConsonant_a1Returns()
        {
            // ARRANGE
            IList<LetterStats> stats = new List<LetterStats>
            {
                new LetterStats
                {
                    Letter = "a",
                    Count = 1
                },
                new LetterStats
                {
                    Letter = "b",
                    Count = 1
                }
            };

            // ACT
            IList<LetterStats> result = _facade.RemoveCharStatsByType(stats, CharType.Consonants);

            // ASSERT
            Assert.That(result.Count == 1);
            Assert.That(result[0].Letter == "a");
            Assert.That(result[0].Count == 1);
        }
    }
}