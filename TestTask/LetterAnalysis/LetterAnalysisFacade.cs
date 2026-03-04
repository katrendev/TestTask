using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Фасад для анализа файлов
    /// </summary>
    internal class LetterAnalysisFacade
    {
        private const string VOWELS = "AEIOUYАОЕЯЁЭЫУИЮaeiouyаоеяёэыуию";

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            ILetterAnalysisStorage storage = new DictionaryLetterStorage();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();

                if (!char.IsLetter(c))
                {
                    continue;
                }

                string letterText = c.ToString();
                storage.Add(letterText);
            }

            return storage.GetStatistics();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            ILetterAnalysisStorage storage = new DictionaryLetterStorage();

            bool hasPair = false;
            char previousChar = ' ';

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                {
                    hasPair = false;
                    continue;
                }

                if (!hasPair)
                {
                    previousChar = char.ToUpper(c);
                    hasPair = true;
                    continue;
                }

                char upperFirstChar = previousChar;
                char upperSecondChar = char.ToUpper(c);
                previousChar = upperSecondChar;

                if (upperFirstChar != upperSecondChar)
                {
                    continue;
                }

                string letterText = string.Concat(upperFirstChar, upperSecondChar);
                storage.Add(letterText);
            }

            return storage.GetStatistics();
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:
                {
                    return letters.Where(x => VOWELS.Contains(x.Letter[0])).ToList();
                }
                case CharType.Vowel:
                {
                    return letters.Where(x => !VOWELS.Contains(x.Letter[0])).ToList();
                }
                default:
                {
                    return letters;
                }
            }
        }
    }
}
