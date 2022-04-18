using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestTask.Stats;

namespace TestTask.Tests.Stats
{
    [TestFixture]
    public class PairLetterStatsCollectorTest
    {
        /// <summary>
        /// Ожидается, что реализация сборщика статистики PairLetterStatsCollector
        /// собирает статистику регистрозависимо и объединяя похожие буквы в латинской и кириллической раскладке,
        /// причём буквы, которых нет в двух алфавитах, исключаются из статистики
        /// </summary>
        [Test]
        public void TestCollection()
        {
            const string testString = "ABCEHKMOPTXY АВСЕНКМОРТХУ abcehkmoptxy авсенкмортху БЮWZ";
            IList<LetterStatItem> result;
            using (var stream = Helpers.StreamFrom(testString))
            {
                result = PairLetterStatsCollector.Instance.Collect(stream);
            }

            void AssertContainsLetterItem(char ch, int count)
            {
                Assert.AreEqual(count, result.FirstOrDefault(item => item.Letter == ch)?.Count);
            }
            Assert.AreEqual(12, result.Count);
            AssertContainsLetterItem('A', 4);
            AssertContainsLetterItem('B', 4);
            AssertContainsLetterItem('C', 4);
            AssertContainsLetterItem('E', 4);
            AssertContainsLetterItem('H', 4);
            AssertContainsLetterItem('K', 4);
            AssertContainsLetterItem('M', 4);
            AssertContainsLetterItem('O', 4);
            AssertContainsLetterItem('P', 4);
            AssertContainsLetterItem('T', 4);
            AssertContainsLetterItem('X', 4);
            AssertContainsLetterItem('Y', 4);
        }
    }
}