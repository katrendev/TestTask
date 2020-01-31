using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestTask.Tests
{
	[TestClass]
	public class ReadOnlyStreamTest
	{
		[TestMethod]
		public void ResetPositionToStart()
		{
			var stream = new ReadOnlyStream("firstFile.txt");

			Assert.AreEqual(stream.GetPosition(), 0);

			stream.ReadNextChar();
			stream.ResetPositionToStart();

			Assert.AreEqual(stream.GetPosition(), 0);
		}

		[TestMethod]
		public void ReadNextChar()
		{
			var stream = new ReadOnlyStream("firstFile.txt");

			var c = stream.ReadNextChar();

			Assert.AreEqual(c, 'Э');
		}
	}
}