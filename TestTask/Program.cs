using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

            try
            {
                IReadOnlyStream inputStream1 = GetInputStream(args[0]);
                IReadOnlyStream inputStream2 = GetInputStream(args[1]);
                inputStream1.ResetPositionToStart();
                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch (System.IndexOutOfRangeException)
            {

                Console.WriteLine("Убедитесь, что входные данные корректны");
            }
            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            System.Console.WriteLine("Нажмите любую клавищу для завершения программы:");
            ConsoleKeyInfo pressedKey = Console.ReadKey();
            while (pressedKey == null)
            {
                pressedKey = Console.ReadKey();
                if (pressedKey != null)
                {
                    return;
                }
            }

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

        private static LetterStats getchcnt(string allFileString, string charString)
        {
            LetterStats letter;
            letter.Letter = charString;
            letter.Count = 0;
            foreach (Match m in Regex.Matches(allFileString, charString))
                letter.Count++;
            return letter;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимы

            // В данном случае коллекции хранятся в списке List, но так же возможно (и, на мой взгляд - более удобно)
            //хранить структуры в коллекции Dictionary<string, Letterstats>, где в качестве ключа - пара или буква,
            //а в качестве значения - структура Letterstats

            IList<LetterStats> singleLetter = new List<LetterStats>();
            try
            {

                IList<string> checker = new List<string>();
                string allFileString = stream.ReadToEnd().ToString();
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                {
                    char nextChar = stream.ReadNextChar();
                    string charString = nextChar.ToString();
                    if (!checker.Contains(charString) & char.IsLetter(nextChar))
                    {
                        singleLetter.Add(getchcnt(allFileString, charString));
                        checker.Add(charString);
                    }
                }
            }
            catch (Exception)
            {
                return singleLetter;
            }
            return singleLetter;

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
            IList<LetterStats> statistics = new List<LetterStats>();
            try
            {

                List<string> checker = new List<string>();
                string allFileString = stream.ReadToEnd().ToString().ToUpper();
                stream.ResetPositionToStart();
                char secondChar = ' ';
                while (!stream.IsEof)
                {
                    char firstChar = char.ToUpper(stream.ReadNextChar());
                    secondChar = char.ToUpper(secondChar);
                    string pair = firstChar.ToString() + secondChar.ToString();
                    if (firstChar.Equals(secondChar) & !checker.Contains(pair) & char.IsLetter(firstChar))
                    {
                        LetterStats pairStruct = getchcnt(allFileString, pair);
                        statistics.Add(pairStruct);
                        checker.Add(pair);
                    }
                    secondChar = firstChar;

                }
            }
            catch (Exception)
            {
                return statistics;
            }
            return statistics;

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
            // TODO : Удалить статистику по запрошенному типу букв.
            string chekline = "";
            List<LetterStats> Newletters = new List<LetterStats>();
            switch (charType)
            {
                case CharType.Consonants:
                    chekline = "бвгджзйклмнпрстфхцчшщъь";
                    break;
                case CharType.Vowel:
                    chekline = "аеёиоуыэюя";
                    break;
            }

            foreach (LetterStats letter in letters)
            {
                if (!chekline.Contains(letter.Letter))
                {
                    Newletters.Add(letter);
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
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            IEnumerable orderedLetters = letters.OrderBy(letter => letter.Letter);
            int counter = 0;
            foreach (LetterStats letter in orderedLetters)
            {
                Console.WriteLine("{" + letter.Letter + "} : {" + letter.Count + "}");
                counter += letter.Count;
            }
            Console.WriteLine("Итого: {0} пар/букв", counter);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.

        /// Поскольку правилами разрешатеся изменение исходной структуры кода,
        /// функция не используется, так как нет необходимости. 
        /// Увеличение счётчика вхождений осуществляется тем же способом
        /// в функции getchcnt(string allFileString, string charString)
        /// </summary>
        /// <param name="letterStats"></param>
        /*private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }*/
    }
}
