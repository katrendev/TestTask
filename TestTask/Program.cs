using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);
            
            inputStream1.FileClose();
            inputStream2.FileClose();

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
            stream.ResetPositionToStart();

            var letterStats = new List<LetterStats>();
            
            while (!stream.IsEof())
            {
                char c = stream.ReadNextChar();

                if (letterStats.Count(x => x.Letter == c.ToString()) != 0)
                {
                    IncStatistic(letterStats.First(x => x.Letter == c.ToString()));
                }
                else
                {
                    letterStats.Add(new LetterStats()
                    {
                        Letter = c.ToString(),
                        Count = 1
                    });
                }
            }

            return letterStats;
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

            var letterStats = new List<LetterStats>();
            
            char d = stream.ReadNextChar();
            while (!stream.IsEof())
            {
                char c = stream.ReadNextChar();

                string checkedSting = (c.ToString() + d).ToUpper();
                
                if (letterStats.Count(x => x.Letter.ToUpper() == checkedSting) != 0)
                {
                    IncStatistic(letterStats.First(x => x.Letter.ToUpper() == checkedSting));
                    stream.ReadNextChar();
                }
                else if(c == d)
                {
                    letterStats.Add(new LetterStats()
                    {
                        Letter = checkedSting,
                        Count = 1
                    });
                }

                d = c;
            }

            return letterStats;
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
            var vowel = "ауоыиэяюёе";
            
            switch (charType)
            {
                case CharType.Consonants:

                    var deletedConsonantsLetters = new List<LetterStats>();
                    
                    foreach (var letter in letters)
                    {
                        if (vowel.ToUpper().All(x => x.ToString()+x != letter.Letter))
                        {
                            deletedConsonantsLetters.Add(letter);
                        }
                    }

                    foreach (var deletedLetter in deletedConsonantsLetters)
                    {
                        letters.Remove(deletedLetter);
                    }
                    break;
                case CharType.Vowel:

                    var deletedVowelLetters = new List<LetterStats>();
                    
                    foreach (var letter in letters)
                    {
                        foreach (var ch in vowel)
                        {
                            if (letter.Letter.Contains(ch.ToString()))
                            {
                                deletedVowelLetters.Add(letter);
                            }
                        }
                    }

                    foreach (var deletedLetter in deletedVowelLetters)
                    {
                        letters.Remove(deletedLetter);
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
            var letterStatsEnumerable = letters as LetterStats[] ?? letters.ToArray();
            var orderedLetters = letterStatsEnumerable.OrderBy(x => x.Letter);
            foreach (var letterStats in orderedLetters)
            {
                Console.WriteLine("{" + letterStats.Letter + "} " + ":" + " {" + letterStats.Count + "}");   
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


    }
}
