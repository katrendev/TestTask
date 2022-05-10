namespace TestTask
{
    internal class Vowel
    {
        private const string Vowels = "AEIOUYАИЕЁОУЫЭЮЯ";

        public static bool IsVowel(string letter)
        {
            return Vowels.IndexOf(letter.ToUpper()) >= 0;
        }
    }
}
