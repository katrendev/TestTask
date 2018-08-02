using System;
using System.Linq;

namespace TestTask
{
    public static class CharExtension
    {
        public static LetterType GetLetterType(this char letter, IAlphabetProvider alphabetProvider)
        {
            if (alphabetProvider == null)
            {
                throw new ArgumentNullException(nameof(alphabetProvider),
                    "Can not determine the letter type: The alphabet is not passed.");
            }

            letter = char.ToLower(letter);
            
            if (!alphabetProvider.Alphabet.Contains(letter))
            {
                throw new Exception(
                    $"Could not determine the character type: {letter}\r\n" +
                    $"This symbol does not belong to the transferred alphabet {nameof(alphabetProvider)}");
            }            
            if (alphabetProvider.Vowels.Contains(letter))
            {
                return LetterType.Vowel;
            }
            if (alphabetProvider.Consonants.Contains(letter))
            {
                return LetterType.Consonant;
            }

            return LetterType.Aphonic;
        }
    }
}