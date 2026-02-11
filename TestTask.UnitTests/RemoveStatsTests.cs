using FluentAssertions;
using System.Collections.Generic;
using TestTask.Helpers;
using TestTask.UnitTests.Fakes;
using TestTask.UnitTests.InputData;
using Xunit;

namespace TestTask.UnitTests
{
    public class RemoveStatsTests
    {
        public static List<object[]> DataWithLettersOnlyVowel = InputObjectData.DataWithLettersOnlyRemoveVowelStat;
        public static List<object[]> DataWithLettersOnlyConsonant = InputObjectData.DataWithLettersOnlyRemoveConsonantStat;

        [Theory]
        [MemberData(nameof(DataWithLettersOnlyVowel))]
        public void after_remove_vowels_single_stats_will_be_have_count(string inputData, int singleStatsCount, int _)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var singleStat = HelperFillStat.FillSingleLetterStats(stream);

            HelperRemoveStat.RemoveCharStatsByType(singleStat, CharType.Vowel);

            singleStat.Should().HaveCount(singleStatsCount);
        }

        [Theory]
        [MemberData(nameof(DataWithLettersOnlyVowel))]
        public void after_remove_vowels_double_stats_will_be_have_count(string inputData, int _, int doubleStatsCount)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var doubleStat = HelperFillStat.FillDoubleLetterStats(stream);

            HelperRemoveStat.RemoveCharStatsByType(doubleStat, CharType.Vowel);

            doubleStat.Should().HaveCount(doubleStatsCount);
        }

        [Theory]
        [MemberData(nameof(DataWithLettersOnlyConsonant))]
        public void after_remove_consonants_single_stats_will_be_have_count(string inputData, int singleStatsCount, int _)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var singleStat = HelperFillStat.FillSingleLetterStats(stream);

            HelperRemoveStat.RemoveCharStatsByType(singleStat, CharType.Consonants);

            singleStat.Should().HaveCount(singleStatsCount);
        }

        [Theory]
        [MemberData(nameof(DataWithLettersOnlyConsonant))]
        public void after_remove_consonants_double_stats_will_be_have_count(string inputData, int _, int doubleStatsCount)
        {
            var stream = new FakeReadOnlyStream(inputData);
            var doubleStat = HelperFillStat.FillDoubleLetterStats(stream);

            HelperRemoveStat.RemoveCharStatsByType(doubleStat, CharType.Consonants);

            doubleStat.Should().HaveCount(doubleStatsCount);
        }
    }
}
