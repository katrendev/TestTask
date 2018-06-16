using System;
using System.Linq;

namespace TestTask
{
    public static class CharExtensions
    {
        public static LetterType GetLetterType(this char letter, IAlphabetProvider alphabetProvider)
        {
            if (alphabetProvider == null)
            {
                throw new ArgumentNullException(nameof(alphabetProvider), 
                    "Невозможно определить тип буквы: Не передан алфавит.");
            }

            letter = char.ToLower(letter);
            
            if (!alphabetProvider.Alphabet.Contains(letter))
            {
                throw new Exception(
                    $"Не удалось определить тип символа: {letter}\r\n" +
                    $"Этот символ не принаджелит переданному алфавиту {nameof(alphabetProvider)}");
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