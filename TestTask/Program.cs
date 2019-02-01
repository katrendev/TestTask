using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> list = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char cRurr = stream.ReadNextChar();
                if (char.IsLetter(cRurr))
                {
                    AddUpdateItem(list, char.ToString(cRurr));
                }
            }

            return list;
        }

        /// <summary>
        /// Метод обновляет (при отсутствии - добавляет) статистику по данному символу
        /// </summary>
        /// <param name="list">Список символов и статистика по ним</param>
        /// <param name="letter">Символ, статистику по которому требуется обновить</param>
        private static void AddUpdateItem(List<LetterStats> list, string letter)
        {
            int index = list.FindIndex(matchItem => matchItem.Letter == letter);
            LetterStats item = index != -1 ? list[index] : new LetterStats() { Letter = letter };
            IncStatistic(ref item);

            if (index != -1)
            {
                list[index] = item;
            }
            else
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> list = new List<LetterStats>();

            stream.ResetPositionToStart();
            char cPrev = char.MinValue;

            while (!stream.IsEof)
            {
                char cCurr = stream.ReadNextChar();
                if (!char.IsLetter(cCurr)) continue;

                cPrev = char.ToUpperInvariant(cPrev);
                cCurr = char.ToUpperInvariant(cCurr);

                if (cPrev == cCurr)
                {
                    string letter = string.Concat(char.ToString(cPrev), char.ToString(cCurr));
                    AddUpdateItem(list, letter);
                    cPrev = char.MinValue;
                }
                else
                {
                    cPrev = cCurr;
                }
            }

            return list;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            List<LetterStats> list = (List<LetterStats>)letters;
            string strToFind = string.Empty;

            switch (charType)
            {
                case CharType.Consonants:
                    strToFind = "[BCDFGHJKLMNPQRSTVWXYZБВГДЖЗЙКЛМНПРСТФХЦЧШЩ]+";
                    break;
                case CharType.Vowel:
                    strToFind = "[AEIOUАОИЕЁЭЫУЮЯ]+";
                    break;
            }

            list.RemoveAll(matchItem =>
            {
                Match m = Regex.Match(matchItem.Letter, strToFind, RegexOptions.IgnoreCase);
                return m.Success;
            });
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            List<LetterStats> list = (List<LetterStats>)letters;
            list.Sort((LetterStats item1, LetterStats item2) => item1.Letter.CompareTo(item2.Letter));

            Console.WriteLine("  Statistics\n--------------");
            foreach (var item in list)
            {
                Console.WriteLine("  Letter: {0},  Count: {1}", item.Letter, item.Count);
            }
            Console.WriteLine(Environment.NewLine);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
