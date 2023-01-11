
using System.Collections.Generic;
using System;
using TestTask.Enums;
using TestTask.Models;
using TestTask.Services.Interfaces;
using System.Linq;

namespace TestTask.Services
{
    internal static class StatisticsService
    {
        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            if (stream == null)
                return new List<LetterStats>();

            var result = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                    continue;

                string s = c.ToString();
                var ls = result.SingleOrDefault(r => r.Letter == s);
                if (ls.Letter == s) result.Remove(ls);
                else ls.Letter = s;

                result.Add(IncStatistic(ls));
            }

            return result;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            if (stream == null)
                return new List<LetterStats>();

            var result = new List<LetterStats>();
            stream.ResetPositionToStart();
            char firstChar = ' ';
            while (!char.IsLetter(firstChar) && !stream.IsEof)
                firstChar = stream.ReadNextChar();
            while (!stream.IsEof)
            {
                char secondChar = stream.ReadNextChar();
                string s = (firstChar.ToString() + secondChar.ToString()).ToUpper();
                if (!char.IsLetter(firstChar) || !char.IsLetter(secondChar) || s[0] != s[1])
                {
                    firstChar = secondChar;
                    continue;
                }
                    
                var ls = result.SingleOrDefault(r => r.Letter == s);

                if (ls.Letter == s) result.Remove(ls);
                else ls.Letter = s;

                result.Add(IncStatistic(ls));
            }
            return result;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            if (letters == null || letters.Count == 0) return;
            const string consonants = "аеёиоуыэюя" +
                "АЕЁИОУЫЭЮЯ" +
                "aeiouy" +
                "AEIOUY";
            const string vowels = "бвгджзйклмнпрстфхцчшщ" +
                "БВГДЖЗЙКЛМНПРСТФХЦЧШЩ" +
                "bcdfghjklmnpqrstvwxyz" +
                "BCDFGHJKLMNPQRSTVWXYZ";
            switch (charType)
            {
                case CharType.Consonants:
                    {
                        for (int i = 0; i < letters.Count; i++)
                        {
                            if (consonants.Contains(letters[i].Letter))
                                letters.RemoveAt(i--);
                        }
                        break;
                    }
                case CharType.Vowel:
                    {
                        for (int i = 0; i < letters.Count; i++)
                        {
                            if (vowels.Contains(letters[i].Letter))
                                letters.RemoveAt(i--);
                        }
                        break;
                    }
            }

        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            foreach (var item in letters.OrderBy(l => l.Letter))
            {
                Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static LetterStats IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
            return letterStats;
        }
    }
}
