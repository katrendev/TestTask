using System;

namespace TestTask
{
    internal static class CharExtensions
	{
        private const string VOWEL = "аеиоуыэюяaeiouy";

		public static bool IsVowel(this char @char)
		{
			return VOWEL.IndexOf(@char.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
		}
	}

}