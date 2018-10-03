using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    internal abstract class LetterStatMaker
    {
        protected IReadOnlyStream stream;
        IList<LetterStats> letterStats;
        public LetterStatMaker(IReadOnlyStream stream)
        {
            this.stream = stream;
            letterStats = FillLetterStats();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик.        
        /// </summary>
        protected abstract IList<LetterStats> FillLetterStats();

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="charType">Тип букв для анализа</param>
        public void RemoveCharStatsByType(CharType charType)
        {
            HashSet<char> vowels = new HashSet<char>();
            foreach (var c in new char[] { 'а', 'о', 'и', 'е', 'ё', 'э', 'ы', 'у', 'ю', 'я' })
            {
                vowels.Add(c);
                vowels.Add(Char.ToUpper(c));
            }

            bool needDelVowel = charType== CharType.Vowel;
            int i = 0;

            while (i < letterStats.Count)
            {
                var letter = letterStats[i].Letter[0];
                if (vowels.Contains(letter) == needDelVowel)
                    letterStats.RemoveAt(i);
                else i++;
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        public void PrintStatistic()
        {
            int total = 0;
            foreach (var l in letterStats.OrderBy(pp => pp.Letter))
            {
                Console.WriteLine($"{l.Letter} : {l.Count}");
                total++;
            }
            Console.WriteLine("ИТОГО: " + total);

        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        public void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
