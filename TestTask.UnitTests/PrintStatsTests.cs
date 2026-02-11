using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TestTask.Helpers;
using TestTask.UnitTests.Fakes;
using TestTask.UnitTests.InputData;
using Xunit;

namespace TestTask.UnitTests
{
    public class PrintStatsTests
    {
        public static List<object[]> DataWithLettersOnlySingle = InputObjectData.DataWithLettersOnlySinglePrintStat;
        public static List<object[]> DataWithLettersOnlyDouble = InputObjectData.DataWithLettersOnlyDoublePrintStat;

        [Theory]
        [MemberData(nameof(DataWithLettersOnlySingle))]
        public void report_single_stats_will_be_have_line_count(string inputData, int singleStatsLineCount)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var singleStat = HelperFillStat.FillSingleLetterStats(stream);
            HelperRemoveStat.RemoveCharStatsByType(singleStat, CharType.Vowel);

            var linesCount = HelperPrintStat.BuildReport(singleStat)
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Count();

            linesCount.Should().Be(singleStatsLineCount);
        }

        [Theory]
        [MemberData(nameof(DataWithLettersOnlyDouble))]
        public void report_double_stats_will_be_have_line_count(string inputData, int doubleStatsLineCount)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var doubleStat = HelperFillStat.FillDoubleLetterStats(stream);
            HelperRemoveStat.RemoveCharStatsByType(doubleStat, CharType.Consonants);

            var linesCount = HelperPrintStat.BuildReport(doubleStat)
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Count();

            linesCount.Should().Be(doubleStatsLineCount);
        }
    }
}
