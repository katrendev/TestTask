using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    public static class Statistics
    {
        #region Public Methods

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        internal static async Task<IList<LetterStats>> FillSingleLetterStatsAsync(IReadOnlyStream stream)
        {
            await stream.ResetPositionToStartAsync();

            IList<LetterStats> letterStatsList = new List<LetterStats>();

            while (!stream.IsEof)
            {
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                char c = await stream.ReadNextCharAsync();
                
                if (c != default)
                {
                    await DoStatisticsAsync(letterStatsList, c.ToString(), StringComparison.InvariantCulture);
                }
            }

            return letterStatsList;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        internal static async Task<IList<LetterStats>> FillDoubleLetterStatsAsync(IReadOnlyStream stream)
        {
            await stream.ResetPositionToStartAsync();

            IList<LetterStats> letterStatsList = new List<LetterStats>();

            char left = default;

            if (!stream.IsEof)
            {
                left = char.ToUpperInvariant(await stream.ReadNextCharAsync());
            }

            while (!stream.IsEof)
            {
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                char right = char.ToUpperInvariant(await stream.ReadNextCharAsync());

                if (left == right)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(left);
                    stringBuilder.Append(right);

                    await DoStatisticsAsync(letterStatsList, stringBuilder.ToString(), StringComparison.InvariantCultureIgnoreCase);
                }

                left = right;
            }

            return letterStatsList;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        internal static Task<IList<LetterStats>> RemoveCharStatsByTypeAsync(IList<LetterStats> letters, CharType charType)
        {
            return Task.Run(() =>
            {
                // TODO : Удалить статистику по запрошенному типу букв.
                string charTypeCollection = default;

                switch (charType)
                {
                    case CharType.Consonants:

                        charTypeCollection = CharTypeStrings.Consonants;
                        break;

                    case CharType.Vowels:

                        charTypeCollection = CharTypeStrings.Vowels;
                        break;
                }

                IList<LetterStats> toDelete = new List<LetterStats>();

                foreach (LetterStats letter in letters)
                {
                    if (charTypeCollection.Contains(letter.Letter[0]))
                    {
                        toDelete.Add(letter);
                    }
                }

                return (IList<LetterStats>)letters.Except(toDelete).ToList();
            });
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        internal static async Task PrintStatisticsAsync(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!

            if (letters.Count() == 0)
            {
                Console.WriteLine();
                Console.WriteLine("No data available.");

                return;
            }

            letters = await SortAsync(letters);

            foreach (LetterStats letterStats in letters)
            {
                Console.WriteLine($"{letterStats.Letter} : {letterStats.Count}");
            }

            int count = await SumAsync(letters);

            Console.WriteLine();
            Console.WriteLine($"Total: {count} {SetUnits(letters.First(), count)}");
            Console.WriteLine();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Sorts a collection in the ascending order.
        /// </summary>
        /// <param name="letters">The collection to sort.</param>
        /// <returns>A new sorted collection.</returns>
        private static Task<IEnumerable<LetterStats>> SortAsync(IEnumerable<LetterStats> letters) =>
            Task.Run(() => (IEnumerable<LetterStats>)letters.OrderBy(s => s.Letter));

        /// <summary>
        /// Returns a total of all letters/pairs occurences.
        /// </summary>
        /// <param name="letters">The collection of letters/pairs.</param>
        /// <returns></returns>
        private static Task<int> SumAsync(IEnumerable<LetterStats> letters) =>
            Task.Run(() => (from letter in letters select letter.Count).Sum());

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Sets data units.
        /// </summary>
        /// <param name="letterStats"><see cref="LetterStats"/>.</param>
        /// <param name="count">The number of letters/pairs found.</param>
        /// <returns></returns>
        private static string SetUnits(LetterStats letterStats, int count)
        {
            string unit = letterStats.Letter.Length == 1 ? "letter" : "pair";
            StringBuilder stringBuilder = new StringBuilder(unit);

            return count > 1 ? stringBuilder.Append("s").ToString() : unit;
        }

        /// <summary>
        /// Does calculations to reflect the current state of the collected data.
        /// </summary>
        /// <param name="letterStatsList">A colletion of <see cref="LetterStats"/> objects.</param>
        /// <param name="datum">A piece of data to collect.</param>
        /// <param name="stringComparison">A method to compare strings.</param>
        /// <returns></returns>
        private static Task DoStatisticsAsync(IList<LetterStats> letterStatsList, string datum, StringComparison stringComparison)
        {
            return Task.Run(() =>
            {
                LetterStats letterStats =
                  letterStatsList.FirstOrDefault(s => datum.Equals(s.Letter, stringComparison));

                if (letterStats == null)
                {
                    letterStats = new LetterStats { Letter = datum };
                    letterStatsList.Add(letterStats);
                }

                IncStatistic(letterStats);
            });
        }

        #endregion
    }
}
