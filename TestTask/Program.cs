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
            IList<LetterStats> singleLetterStats;
            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            {
                singleLetterStats = FillSingleLetterStats(inputStream1);                
            }
            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            PrintStatistic(singleLetterStats);

            Console.WriteLine(new string('-', 20));

            IList<LetterStats> doubleLetterStats;
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2);
            }
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
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
            List<LetterStats> listLetter = new List<LetterStats>();
            
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!char.IsLetter(c))
                    continue;

                LetterStats letter = listLetter.FirstOrDefault(ch => ch.Letter.Equals(c));

                if (letter == null)
                    listLetter.Add(new LetterStats(c));

                else
                    IncStatistic(letter);
            }
            return listLetter;
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
            List<LetterStats> listLetter = new List<LetterStats>();

            stream.ResetPositionToStart();

            char tempPosition = default;
            bool isFirst = true;
            while (!stream.IsEof)
            {
                char c = char.ToLower(stream.ReadNextChar());
                if (!char.IsLetter(c) || isFirst)
                {
                    tempPosition = c;
                    if (isFirst)
                        isFirst = false;

                    continue;

                }
                if (c.Equals(tempPosition))
                {
                    LetterStats letter = listLetter.FirstOrDefault(ch => ch.Letter.Equals(c));

                    if (letter == null)
                        listLetter.Add(new LetterStats(c));
                    else
                        IncStatistic(letter);
                }
                tempPosition = c;
            }
            return listLetter;
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
            List<char> vowelsList = new List<char>() { 'а', 'о', 'ы', 'и', 'у', 'э', 'ё', 'я', 'е', 'ю' };
            switch (charType)
            {                
                case CharType.Consonants:
                    for (int i = letters.Count - 1; i >= 0 ; i--)
                        if (!vowelsList.Contains(char.ToLower(letters[i].Letter)))
                            letters.Remove(letters[i]);
                    break;

                case CharType.Vowel:
                    for (int i = letters.Count - 1; i >= 0; i--)
                        if (vowelsList.Contains(char.ToLower(letters[i].Letter)))
                            letters.Remove(letters[i]);
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
            var sortLetters = letters.OrderBy(item => item.Letter).ToList();
            foreach (var stat in sortLetters)
                Console.WriteLine($"Letter: {stat.Letter}, Count: {stat.Count}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }


    }
}
