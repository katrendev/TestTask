using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestTask.Stats;

namespace TestTask.Tests.Stats
{
    [TestFixture]
    public class LetterStatsCollectorTest
    {
        /// <summary>
        /// Ожидается, что базовая реализация сборщика статистики собирает статистику по всм символам, не только буквам
        /// </summary>
        [Test]
        public void TestCollection()
        {
            const string testString = "АААааббв1+";
            IList<LetterStatItem> result;
            using (var stream = Helpers.StreamFrom(testString))
            {
                result = new LetterStatsCollector().Collect(stream);
            }

            void AssertContainsLetterItem(char ch, int count)
            {
                Assert.AreEqual(count, result.FirstOrDefault(item => item.Letter == ch)?.Count);
            }
            Assert.AreEqual(6, result.Count);
            AssertContainsLetterItem('А', 3);
            AssertContainsLetterItem('а', 2);
            AssertContainsLetterItem('б', 2);
            AssertContainsLetterItem('в', 1);
            AssertContainsLetterItem('1', 1);
            AssertContainsLetterItem('+', 1);
        }
    }
}