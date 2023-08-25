using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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

            Console.WriteLine("first output:");
            PrintStatistic(singleLetterStats);
            Console.WriteLine("second output:");
            PrintStatistic(doubleLetterStats);

            inputStream1.CloseStream();
            inputStream2.CloseStream();

            Console.WriteLine("Press key to continue.");
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
            var letterStatsDictionary = new Dictionary<string, LetterStats>();

            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                {
                    continue;
                }
                
                UpdateLettersStats(c.ToString(), letterStatsDictionary);
            }

            return new List<LetterStats>(letterStatsDictionary.Values);
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
            char previousChar;
            var lettersStatsList = new List<LetterStats>();

            if (stream.IsEof)
            {
                return lettersStatsList;
            }
            previousChar = Char.ToLower(stream.ReadNextChar());

            var letterStatsDictionary = new Dictionary<string, LetterStats>();
            while (!stream.IsEof)
            {
                char c = Char.ToLower(stream.ReadNextChar());
                if (!Char.IsLetter(previousChar) || !Char.IsLetter(c) || previousChar != c)
                {
                    previousChar = c;
                    continue;
                }
            
                var key = new string(new char[] {c, c});
                UpdateLettersStats(key, letterStatsDictionary);
                previousChar = c;
            }

            return new List<LetterStats>(letterStatsDictionary.Values);
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
            var charMaster = new CharMaster();
            switch (charType)
            {

                case CharType.Consonants:
                    for (int i = 0; i < letters.Count;)
                    {
                        var letterStat = letters[i];
                        if (letterStat.Letter.Length == 0 || charMaster.IsConsonants(letterStat.Letter[0]))
                        {
                            letters.RemoveAt(i);
                        }
                        else
                        {
                            ++i;
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.Count;)
                    {
                        var letterStat = letters[i];
                        if (letterStat.Letter.Length == 0 || charMaster.IsVowel(letterStat.Letter[0]))
                        {
                            letters.RemoveAt(i);
                        }
                        else
                        {
                            ++i;
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
            var sortedList = GetSortedLettersStatsList(letters);

            var stringBuilder = new StringBuilder();
            foreach(var letterStat in sortedList)
            {
                stringBuilder.Clear();
                stringBuilder.Append('{');
                stringBuilder.Append(letterStat.Letter[0]);
                stringBuilder.Append("} : {");
                stringBuilder.Append(letterStat.Count);
                stringBuilder.Append('}');
                Console.WriteLine(stringBuilder.ToString());
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }

        private static void UpdateLettersStats(string key, Dictionary<string, LetterStats> letterStatsDictionary)
        {
            if (letterStatsDictionary.ContainsKey(key))
            {
                var currentLetterStats = letterStatsDictionary[key];
                currentLetterStats.Count++;
                letterStatsDictionary[key] = currentLetterStats;
            }
            else
            {
                var newLetterStats = new LetterStats() { Letter=key, Count=1 };
                letterStatsDictionary.Add(key, newLetterStats);
            }
        }

        private static IList<LetterStats> GetSortedLettersStatsList(IEnumerable<LetterStats> letters)
        {
            var sortedList = new List<LetterStats>();

            foreach (var letterStat in letters)
            {
                int i = 0;
                bool insertedElement = false;
                for (; i < sortedList.Count; ++i)
                {
                    var currentLetter = letterStat.Letter;
                    var currentLetterFromSorted = sortedList[i].Letter;
                    if(currentLetter.Length == 0 || 
                        Char.ToLower(currentLetter[0]) <= Char.ToLower(currentLetterFromSorted[0])
                    )
                    {
                        sortedList.Insert(i, letterStat);
                        insertedElement = true;
                        break;
                    }
                }
                if(!insertedElement)
                {
                    sortedList.Add(letterStat);
                }
            }

            return sortedList;
        }
    }
}
