using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        // Массивы, хранящие гласные и согласные буквы для проверки
        private static readonly char[] vowels = new char[] { 'а', 'о', 'и', 'е', 'ё', 'э', 'ы', 'у', 'ю', 'я' };
        private static readonly char[] consonants = new char[] { 'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

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

            CloseInputStream(inputStream1);
            CloseInputStream(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Нажмите любую клавишу для завершения работы программы");
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
            stream.ResetPositionToStart();

            List<LetterStats> letterStatsList = new List<LetterStats>();

            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();

                    // Проверяем, что символ является буквой, иначе продолжаем проход по циклу
                    if (!Char.IsLetter(c))
                    {
                        continue;
                    }

                    AddCharInListWithCheck(letterStatsList, c.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();

            List<LetterStats> letterStatsList = new List<LetterStats>();

            // Переменная для хранения второго символа
            string secondChar = default;

            if (!stream.IsEof)
            {
                secondChar = stream.ReadNextChar().ToString().ToLower();
            }

            while (!stream.IsEof)
            {
                try
                {
                    string firstChar = secondChar.ToLower();
                    secondChar = stream.ReadNextChar().ToString().ToLower();

                    // Проверяем, что символ является буквой, иначе продолжаем проход по циклу
                    if (!Char.IsLetter(firstChar, 0))
                    {
                        continue;
                    }

                    if (firstChar.Equals(secondChar))
                    {
                        string charsPair = string.Concat(firstChar, secondChar);
                        AddCharInListWithCheck(letterStatsList, charsPair);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
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
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // Выбираем необходимый массив (гласных или согласных букв)
            var usedCharsTypeArray = charType == CharType.Vowel ? vowels : consonants;

            int index = 0;

            while (index < letters.Count)
            {
                var currentLetter = letters[index];

                if (usedCharsTypeArray.Contains(currentLetter.Letter.ToLower().First()))
                {
                    letters.Remove(currentLetter);
                    continue;
                }

                index++;
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IList<LetterStats> letters)
        {
            var sortedLetters = letters.OrderBy(l => l.Letter);
            var sum = 0;

            foreach (var letter in sortedLetters)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
                sum += letter.Count;
            }

            Console.WriteLine($"Итого : {sum}");
            Console.WriteLine();
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Метод необходимой проверки строки на подобные вхождения в списке.
        /// Добавляет новый элемент в список, если отсутствуют подобные, выставляя счетчику единицу.
        /// Иначе вызывает функцию инкрементирования.
        /// </summary>
        /// <param name="letterStatsList">Список статистики вхождений</param>
        /// <param name="charString">Строка из символа или нескольких символов</param>
        private static void AddCharInListWithCheck(List<LetterStats> letterStatsList, string charString)
        {
            var foundedLetterStatsElement = letterStatsList.Find(ls => ls.Letter.Equals(charString));

            if (foundedLetterStatsElement == null)
            {
                letterStatsList.Add(new LetterStats()
                {
                    Letter = charString,
                    Count = 1
                });
            }
            else
            {
                IncStatistic(foundedLetterStatsElement);
            }
        }

        /// <summary>
        /// Метод для закрытия потока чтения
        /// </summary>
        /// <param name="readOnlyStream">Поток</param>
        private static void CloseInputStream(IReadOnlyStream readOnlyStream)
        {
            readOnlyStream.Close();
        }
    }
}