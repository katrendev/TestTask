using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TestTask.Models;
using TestTask.Services;

namespace TestTask.Tests
{
	[TestClass]
	public class LettersAnalyzerTest
	{
		private IReadOnlyStream GetStream()
		{
			return new ReadOnlyStream("firstFile.txt");
		}

		[TestMethod]
		public void GetInputStream()
		{
			var stream = GetStream();

			Assert.IsNotNull(stream);
		}

		[TestMethod]
		public void FillSingleLetterStats()
		{
			var stream = GetStream();

			var stats = LettersAnalyzer.FillSingleLetterStats(stream);

			Assert.AreEqual(stats.Count, 10);
			Assert.AreEqual(stats[0].Count, 1);
			Assert.AreEqual(stats[4].Count, 2);
		}

		[TestMethod]
		public void FillDoubleLetterStats()
		{
			var stream = GetStream();

			var stats = LettersAnalyzer.FillDoubleLetterStats(stream);

			Assert.AreEqual(stats.Count, 5);
			Assert.AreEqual(stats[1].Letter, "ББ");
			Assert.AreEqual(stats[2].Count, 1);
		}

		[TestMethod]
		public void RemoveCharStatsByType()
		{
			var stream = GetStream();
			var stats = LettersAnalyzer.FillSingleLetterStats(stream);

			LettersAnalyzer.RemoveCharStatsByType(stats, CharType.Vowel);

			Assert.AreEqual(stats.Count(x => x.Letter == "Э"), 0);
			Assert.AreEqual(stats.Count(x => x.Letter == "Г"), 1);

			stream.ResetPositionToStart();
			stats = LettersAnalyzer.FillSingleLetterStats(stream);

			LettersAnalyzer.RemoveCharStatsByType(stats, CharType.Consonants);

			Assert.AreEqual(stats.Count(x => x.Letter == "Э"), 0);
			Assert.AreEqual(stats.Count(x => x.Letter == "Г"), 0);
		}
	}
}