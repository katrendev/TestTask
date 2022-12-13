namespace TestTask
{
    internal static class LetterStatsExtensions
	{
		public static bool IsVowel(this LetterStats letterStats)
		{
			return letterStats.Letter[0].IsVowel();
		}
	}
}
