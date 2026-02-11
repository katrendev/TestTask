using System.Collections.Generic;
using System.Linq;

namespace TestTask.Helpers
{
    public static class HelperRemoveStat
    {
        private static readonly HashSet<char> __vowelsEnRu = 
            new HashSet<char>("aeiouAEIOUаеёиоуыэюяАЕЁИОУЫЭЮЯ".ToCharArray());

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв. (+)
            switch (charType)
            {
                case CharType.Consonants:
                    RemoveConsonants(letters);
                    break;
                case CharType.Vowel:
                    RemoveVowels(letters);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Метод удаляет статистику по гласным буквам.
        /// </summary>
        /// <param name="letters">Статистика.</param>
        private static void RemoveVowels(IList<LetterStats> letters)
        {
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                if (__vowelsEnRu.Contains(letters[i].Letter.First()))
                    letters.RemoveAt(i);
            }
        }

        /// <summary>
        /// Метод удаляет статистику по согласным буквам.
        /// </summary>
        /// <param name="letters">Статистика.</param>
        private static void RemoveConsonants(IList<LetterStats> letters)
        {
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                if (!__vowelsEnRu.Contains(letters[i].Letter.First()))
                    letters.RemoveAt(i);
            }
        }
    }
}
