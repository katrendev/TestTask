namespace TestTaskTests;

using TestTask;

[TestClass]
public class ProgramTests
{
    private class TestReader(string text) : IReadOnlyStream
    {
        private int _index = 0;
        private readonly string _text = text;

        public bool IsEof
        {
            get
            {
                return _index >= _text.Length;
            }                    
        }

        public void Dispose()
        {
        }

        public char ReadNextChar()
        {
            return _text[_index++];
        }

        public void ResetPositionToStart()
        {
            _index = 0;
        }
    }

    [TestMethod]
    public void FillSingleLetterStats_CountsLetterCaseSensitive()
    {
        var stats = Program.FillSingleLetterStats(new TestReader("aAa"));
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(2, stats[0].Count);
        Assert.AreEqual(1, stats[1].Count);
    }

    [TestMethod]
    public void FillSingleLetterStats_SortsLetterAlphabetically()
    {
        var stats = Program.FillSingleLetterStats(new TestReader("aяa"));
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual("a", stats[0].Letter);
        Assert.AreEqual("я", stats[1].Letter);
    }

    /// <summary>
    /// –езультат не должен быть фиксированного размера (не Array),
    /// т.к. мы собираемс€ его потом фильтровать по гласным-согласным
    /// </summary>
    [TestMethod]
    public void FillSingleLetterStats_GivesResizeableContainer()
    {
        var stats = Program.FillSingleLetterStats(new TestReader("aяa"));
        stats.RemoveAt(0);
    }

    [TestMethod]
    public void FillDoubleLetterStats_CountsLetterCaseInsensitive()
    {
        var stats = Program.FillDoubleLetterStats(new TestReader("aAAaBb"));
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(2, stats[0].Count);
        Assert.AreEqual(1, stats[1].Count);
    }

    [TestMethod]
    public void FillDoubleLetterStats_CountsOnlyPairs()
    {
        var stats = Program.FillDoubleLetterStats(new TestReader("aAAбЅб"));
        Assert.AreEqual(2, stats.Count);
        Assert.AreEqual(1, stats[0].Count);
        Assert.AreEqual(1, stats[1].Count);
    }

    [TestMethod]
    public void FillDoubleLetterStats_SortsLetterAlphabetically()
    {
        var stats = Program.FillDoubleLetterStats(new TestReader("я€ddЅЅ"));
        Assert.AreEqual(3, stats.Count);
        Assert.AreEqual("dd", stats[0].Letter);
        Assert.AreEqual("бб", stats[1].Letter);
        Assert.AreEqual("€€", stats[2].Letter);
    }

    /// <summary>
    /// –езультат не должен быть фиксированного размера (не Array),
    /// т.к. мы собираемс€ его потом фильтровать по гласным-согласным
    /// </summary>
    [TestMethod]
    public void FillDoubleLetterStats_GivesResizeableContainer()
    {
        var stats = Program.FillDoubleLetterStats(new TestReader("aa"));
        stats.RemoveAt(0);
    }

    [TestMethod]
    public void RemoveCharStatsByType_HandlesVowels()
    {
        var stats = new List<LetterStats>
        {
            new() { Letter = "a" },
            new() { Letter = "AA" },
            new() { Letter = "б" },
        };
        Program.RemoveCharStatsByType(stats, CharType.Vowel);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual("б", stats.First().Letter);
    }

    [TestMethod]
    public void RemoveCharStatsByType_HandlesConsonants()
    {
        var stats = new List<LetterStats>
        {
            new() { Letter = "б" },
            new() { Letter = "e" },
            new() { Letter = "D" },
        };
        Program.RemoveCharStatsByType(stats, CharType.Consonants);
        Assert.AreEqual(1, stats.Count);
        Assert.AreEqual("e", stats.First().Letter);
    }
}
