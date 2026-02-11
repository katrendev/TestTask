using FluentAssertions;
using System.Collections.Generic;
using TestTask.Helpers;
using TestTask.UnitTests.InputData;
using Xunit;

namespace TestTask.UnitTests
{
    public class HelperMainTests
    {
        public static List<object[]> DataWithBadPaths = InputObjectData.DataWithBadPaths;

        [Theory]
        [MemberData(nameof(DataWithBadPaths))]
        public void isBadArgs_with_bad_args_will_be_true(string[] args)
        {
            var result = HelperMain.IsBadArgs(args);

            result.Should().BeTrue();
        }
    }
}
