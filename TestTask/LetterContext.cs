using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    /// <summary>
    /// Содержит логику работы с <see cref="LetterStats"/>. 
    /// </summary>
    public class LetterContext
    {
        private IList<char> _vowel = new List<char>()
            {'а', 'е', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я', 'a', 'e', 'i', 'o', 'u', 'y'};

        private IList<LetterStats> _list;

        public LetterContext(IList<LetterStats> list)
        {
            _list = list;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        public void RemoveCharStatsByType(CharType charType)
        {
            switch (charType)
            {
                case CharType.Consonants:
                    _list = _list.Where(x => !_vowel.Contains(x.Letter.ToLower()[0])).ToList();
                    break;
                case CharType.Vowel:
                    _list = _list.Where(x => _vowel.Contains(x.Letter.ToLower()[0])).ToList();
                    break;
                default:
                    throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        public void PrintStatistic()
        {
            IEnumerable<LetterStats> sortLet = _list.OrderBy(x => x.Letter);
            foreach (var item in sortLet)
                Console.WriteLine(item);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        public static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}