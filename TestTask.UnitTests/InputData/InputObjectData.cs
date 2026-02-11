using System.Collections.Generic;

namespace TestTask.UnitTests.InputData
{
    public static class InputObjectData
    {
        private static readonly List<string> _inputData = new List<string>()
        {
            "ААБББВВВВГДЕЖЗЯЯЯ",
            "аабббввввгдежзяяя",
            "аАБбБвВВвГдЕЖзяЯЯ",
        };


        // format: inputData
        public static List<object[]> DataWithNoLetters = new List<object[]>()
        {
            new object[] { "" },
            new object[] { "123454321708090706,-./" },
            new object[] { "12345\0\0\0\0\0\0\0\0\0\0\t\t\t\r\n\r\n"},
        };

        // format: inputData, singleStatCount, doubleStatCount
        public static List<object[]> DataWithLettersOnlyFillStat = new List<object[]>()
        {
            new object[] { _inputData[0], 9, 4 },
            new object[] { _inputData[1], 9, 4 },
            new object[] { _inputData[2], 13, 4},
        };

        // format: inputData, singleStatCount, doubleStatCount
        public static List<object[]> DataWithLettersOnlyRemoveVowelStat = new List<object[]>()
        {
            new object[] { _inputData[0], 6, 2 },
            new object[] { _inputData[1], 6, 2 },
            new object[] { _inputData[2], 8, 2 },
        };

        // format: inputData, singleStatCount, doubleStatCount
        public static List<object[]> DataWithLettersOnlyRemoveConsonantStat = new List<object[]>()
        {
            new object[] { _inputData[0], 3, 2 },
            new object[] { _inputData[1], 3, 2 },
            new object[] { _inputData[2], 5, 2 },
        };


        // format: inputData, singleStatLineCount
        public static List<object[]> DataWithLettersOnlySinglePrintStat = new List<object[]>()
        {
            new object[] { _inputData[0], 7 },
            new object[] { _inputData[1], 7 },
            new object[] { _inputData[2], 9 },
        };

        // format: inputData, doubleStatLineCount
        public static List<object[]> DataWithLettersOnlyDoublePrintStat = new List<object[]>()
        {
            new object[] { _inputData[0], 3 },
            new object[] { _inputData[1], 3 },
            new object[] { _inputData[2], 3 },
        };

        // format: args[]
        public static List<object[]> DataWithBadPaths = new List<object[]>()
        {
            new object[] { null },
            new object[] { new object[] { null, null } },
            new object[] { new object[] { "", "" } },
            new object[] { new object[] { null, "" } },
            new object[] { new object[] { "", null } },
            new object[] { new object[] { null, "1" } },
            new object[] { new object[] { "1", null } },
            new object[] { new object[] { "", "1" } },
            new object[] { new object[] { "1", "" } },
            new object[] { new object[] { "1", "" } },
            new object[] { new object[] { "1" } },
            new object[] { new object[] { "" } },
            new object[] { new object[] { null } },
        };
    }
}
