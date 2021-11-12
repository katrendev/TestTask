using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Text;

namespace TestTask
{
    public class Program
    {
        #region Constants
        private const string VOWELS = "аеёиоуыэюяaeiou";
        private const string CONSONANTS = "йцкнгшщзхфвпрлджбтмсчbcdfghjklmnpqrstvwxyz";
        #endregion

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
                FilesCreator();

                using (IReadOnlyStream inputStream1 = GetInputStream("11.txt"))
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    PrintStatistic(singleLetterStats);
                }

                Console.WriteLine("------------------------------------------------------------------------------------");

                using (IReadOnlyStream inputStream2 = GetInputStream("12.txt"))
                {
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);
                    PrintStatistic(doubleLetterStats);
                }
            }
            catch
            {
                throw new EndOfStreamException();
            }

            Console.ReadKey();
        }

        private static void FilesCreator()
        {
            var file1 = File.Create($"{Environment.CurrentDirectory}/11.txt");

            StreamWriter writer1 = new StreamWriter(file1,Encoding.UTF8);
            writer1.WriteLine("AAAAAAdfgskgsjgspepVSDJGPBMSEOPGEJEGJjsgpsegjEGSBXBXXAAFAEGSGmjjgdfAFABASfsdgssgkp");
            writer1.Close();
            file1.Close();

            var file2 = File.Create($"{Environment.CurrentDirectory}/12.txt");

            StreamWriter writer2 = new StreamWriter(file2, Encoding.UTF8);
            writer2.WriteLine("AADDSSQQwwzzbbhdhhdjAAZZghhAFAFGsgjjdgdfojAFADFAsdggfsASFFGdhkphd");
            writer2.Close();
            file2.Close();
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
            IDictionary<char,LetterStats> letters = new Dictionary<char,LetterStats>();

            while (!stream.IsEof)
            {
                if(letters != null)
                {
                    var c = stream.ReadNextChar();
                    var letter = new LetterStats(c.ToString());

                    if (letters.ContainsKey(c))
                    {
                        letter = letters[c];
                    }
                    else
                    {
                        letters.Add(c,letter);
                    }

                    IncStatistic(letter);
                }
            }

            return letters.Values.ToList();
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
            IDictionary<string,LetterStats> letters = new Dictionary<string,LetterStats>();
            var doubleSymbols = "";

            while (!stream.IsEof)
            {
                if (letters != null)
                {
                    var isContains = false;
                    var c = stream.ReadNextChar();
                    var letter = new LetterStats();

                    doubleSymbols += c;

                    if (letters.ContainsKey(doubleSymbols.ToLower()))
                    {
                        letter = letters[doubleSymbols.ToLower()];
                        isContains = true;
                    }

                    if (doubleSymbols.Length == 2)
                    {
                        doubleSymbols = doubleSymbols.ToLower();

                        if (doubleSymbols[0].Equals(doubleSymbols[1]))
                        {
                            if (!isContains)
                            {
                                letter.Letter = doubleSymbols;
                                letters.Add(letter.Letter, letter);
                            }

                            IncStatistic(letter);
                        }

                        doubleSymbols = c.ToString();
                    }
                }
            }

            return letters.Values.ToList();
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
            var removedElements = "";

            switch (charType)
            {
                case CharType.Consonants:
                    removedElements = CONSONANTS;
                    break;
                case CharType.Vowel:
                    removedElements = VOWELS;
                    break;
            }

            var regex = new Regex($@"^[{removedElements}]*$",RegexOptions.IgnoreCase);

            for(var i = 0; i < letters.Count; i++)
            {
                if (regex.IsMatch(letters[i].Letter))
                {
                    letters.Remove(letters[i]);
                    i--;
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
            var nums = 0;

            letters = letters.Select(letter => letter).OrderBy(letter => letter.Letter);

            foreach(var letter in letters)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
                nums++;
            }

            Console.WriteLine($"TOTAL: {nums}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letter"></param>
        private static void IncStatistic(LetterStats letter) => letter.Count++;
    }
}