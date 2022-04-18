using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestTask.Stats;

namespace TestTask.Tests.Stats
{
    [TestFixture]
    public class SimpleLetterStatsCollectorTest
    {
        /// <summary>
        /// Ожидается, что реализация сборщика статистики SimpleLetterStatsCollector
        /// собирает статистику регистроНЕзависимо и только по буквам
        /// </summary>
        [Test]
        public void TestCollection()
        {
            const string testString = "АааWww123+-*/%$^&#";
            IList<LetterStatItem> result;
            using (var stream = Helpers.StreamFrom(testString))
            {
                result = SimpleLetterStatsCollector.Instance.Collect(stream);
            }

            void AssertContainsLetterItem(char ch, int count)
            {
                Assert.AreEqual(count, result.FirstOrDefault(item => item.Letter == ch)?.Count);
            }
            Assert.AreEqual(4, result.Count);
            AssertContainsLetterItem('А', 1);
            AssertContainsLetterItem('а', 2);
            AssertContainsLetterItem('W', 1);
            AssertContainsLetterItem('w', 2);
        }
    }
}