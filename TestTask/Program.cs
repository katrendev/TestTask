using System;

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
            if (args.Length == 2)
            {
                string firstFilePath = args[0];
                string secondFilePath = args[1];

                var singleLetterTextScanner = new TextScanner(firstFilePath)
                {
                    CaseSensitive = true,
                    SearchKind = SearchKind.SingleLetters
                };
                var doubleLetterTextScanner = new TextScanner(secondFilePath)
                {
                    CaseSensitive = false,
                    SearchKind = SearchKind.DoubleLetters
                };

                // считываем из входящего потока все буквы и возвращаем статистику вхождения каждой буквы (статистика РЕГИСТРОЗАВИСИМАЯ)
                var singleLetterSearchResult = singleLetterTextScanner.Search();
                // считываем из входящего потока все буквы и возвращаем статистику вхождения парных букв (статистика НЕ регистрозависимая)
                var doubleLetterSearchResult = doubleLetterTextScanner.Search();

                // исключаем из статистики вхождения каждой буквы данные по всем гласным
                singleLetterSearchResult.ExcludeLettersByCharType(CharType.Vowel);
                // исключаем из статистики вхождения парных букв данные по всем согласным
                doubleLetterSearchResult.ExcludeLettersByCharType(CharType.Consonants);

                PrintStatistic(singleLetterSearchResult);
                PrintStatistic(doubleLetterSearchResult);
            }
            else
            {
                Console.WriteLine("Приложение запущено некорректно - в аргументах запуска приложения необходимо указать два пути к текстовым файлам");
            }

            // Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="searchResult">результат со статистикой поиска</param>
        private static void PrintStatistic(SearchResult searchResult)
        {
            Console.WriteLine(searchResult.ToString());
        }
    }
}
