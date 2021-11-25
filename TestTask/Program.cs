using System;
using System.Collections.Generic;
using System.Linq;

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
            IReadOnlyStream inputStream1 = GetInputStream(@"TestText1.txt");
            IReadOnlyStream inputStream2 = GetInputStream(@"TestText2.txt");

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            Console.ReadKey();
            Console.Clear();

            PrintStatistic(doubleLetterStats);
            Console.ReadKey();
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
            
            List<LetterStats> stats = new List<LetterStats>();
            string symbol;
            bool addSymbol;

            while (!stream.IsEof)
            {
                symbol = stream.ReadNextChar().ToString();
                addSymbol = true;

                for (int i = 0; i < stats.Count; i++)
                {
                    if (stats[i].Letter == symbol)
                    {
                        IncStatistic(stats , i);
                        addSymbol = false;
                    }
                }

                if (addSymbol)
                {
                    LetterStats letterStats = new LetterStats() { Letter = symbol , Count = 1};
                    stats.Add(letterStats);
                }
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
            }

            return stats;
            throw new NotImplementedException();
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

            List<LetterStats> stats = new List<LetterStats>();
            string symbols = "";
            string firstSymbol;
            string secondSymbol = "";
            bool addSymbols;

            while (!stream.IsEof)
            {
                if (symbols == "")
                {
                    firstSymbol = stream.ReadNextChar().ToString().ToUpper();
                    if (!stream.IsEof)
                    {
                        secondSymbol = stream.ReadNextChar().ToString().ToUpper();
                    }
                }

                else
                {
                    firstSymbol = secondSymbol;
                    secondSymbol = stream.ReadNextChar().ToString().ToUpper();
                }

                symbols = firstSymbol + secondSymbol;
                addSymbols = true;

                for (int i = 0; i < stats.Count; i++)
                {
                    if (stats[i].Letter == symbols)
                    {
                        IncStatistic(stats, i);
                        addSymbols = false;
                        symbols = "";
                    }
                }

                if (addSymbols && symbols != "" && firstSymbol == secondSymbol)
                {
                    LetterStats letterStats = new LetterStats() { Letter = symbols, Count = 1 };
                    stats.Add(letterStats);
                    symbols = "";
                }
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }

            return stats;
            throw new NotImplementedException();
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
            string vowels = "ЁУЕЫАОЭЯИЮQEYUIOA";
            string consonants = "ЙЦКНГШЩЗХЪФВПРЛДЖЧСМТЬБWRTPSDFGHJKLZXCVBNM";
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = letters.Count - 1; i > -1; i--)
                    {
                        if (!vowels.Contains(letters[i].Letter.Substring(1).ToUpper()))
                        {
                            letters.Remove(letters[i]);
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (int i = letters.Count -1; i > -1; i--)
                    {
                        if (!consonants.Contains(letters[i].Letter.ToUpper()))
                        {
                            letters.Remove(letters[i]);
                        }
                    }
                    break;
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
            var newLetters = letters.OrderBy(x => x.Letter);
            int totalNumberOfLetters = 0;

            foreach (var letter in newLetters)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
                totalNumberOfLetters += letter.Count;
            }

            Console.WriteLine($"\nОбщее количество найденных букв/пар: {totalNumberOfLetters}");
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(List<LetterStats> letterStats, int i)
        {
            letterStats[i].Count++;
        }
    }
}
