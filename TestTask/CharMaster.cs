namespace TestTask
{
    public class CharMaster
    {
        private readonly string Vowel = "eyuioa";
        private readonly string Consonants = "qwrtpsdfghjklzxcvbnm";

        public bool IsVowel(char c)
        {
            return Vowel.Contains(Char.ToLower(c));
        }
        public bool IsConsonants(char c)
        {
            return Consonants.Contains(Char.ToLower(c));
        }
    }
}