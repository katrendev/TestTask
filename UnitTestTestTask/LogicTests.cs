using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    // The test class
    [TestClass]
    public class LogicTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var testItems = new List<char>() { 'A', 'Б', 'D', 'Y', 'A' };

            IReadOnlyStream stream = new ReadOnlyStream();
            stream.Items = testItems;

            IStatisticsCreator logics = new StatisticsCreator();
            var result = logics.FillSingleLetterStats(stream);

            var condition = result.Where(x => x.Letter == ('A').ToString()).FirstOrDefault();
            Assert.IsTrue(condition != null);
            Assert.AreEqual(2, condition.Count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var testItems = new List<char>() { 'A', 'А', 'D', 'Y', 'A' };

            IReadOnlyStream stream = new ReadOnlyStream();
            stream.Items = testItems;

            IStatisticsCreator logics = new StatisticsCreator();
            var result = logics.FillDoubleLetterStats(stream);

            var condition = result.Where(x => x.Letter == "AА").FirstOrDefault();
            Assert.IsTrue(condition != null);
            Assert.AreEqual(1, condition.Count);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var testItems = new List<char>() { 'A', 'А', 'D', 'Y', 'A' };

            IReadOnlyStream stream = new ReadOnlyStream();
            stream.Items = testItems;

            IStatisticsCreator logics = new StatisticsCreator();
            var result = logics.FillDoubleLetterStats(stream);

            var excludeList = new List<string>() { "Y" };
            var list = (logics as StatisticsCreator).RemoveCharStatsByType(result, excludeList);

            var condition = list.Where(x => x.Letter.Contains("Y")).FirstOrDefault();
            Assert.IsTrue(condition == null);
        }
    }
}
