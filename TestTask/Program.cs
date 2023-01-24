using System;
using System.Collections;
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
        /// Из первого набора отфильтровываются только гласные
        /// Из второго набора отфильтровываются только согласные
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

            singleLetterStats = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            doubleLetterStats = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Press any key to continue...");
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
            IList<LetterStats> singleLetterStats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                try {
                    string readString = stream.ReadNextChar().ToString();
                    LetterStats selectedLetterStat = singleLetterStats.Where(p => p.Letter == readString).FirstOrDefault();
                    CheckExistStatistic(singleLetterStats, selectedLetterStat, readString);
                    IncStatistic(selectedLetterStat);
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
        }
            return singleLetterStats;
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
            IList<LetterStats> singleLetterStats = new List<LetterStats>();
            PairFounder pairFounder = new PairFounder();
            string pair;

            while (!stream.IsEof)
            {
                try {
                    string readString = stream.ReadNextChar().ToString();
                    if (stream.IsEof)
                    {
                        pair = pairFounder.CheckLetterLast(readString);
                    }
                    else
                    {
                        pair = pairFounder.CheckLetter(readString);
                    }
                    if (pair != null)
                    {
                        LetterStats selectedLetterStat = singleLetterStats.Where(p => p.Letter == pair).FirstOrDefault();
                        CheckExistStatistic(singleLetterStats, selectedLetterStat, pair);
                        IncStatistic(selectedLetterStat);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
            return singleLetterStats;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letterStats, CharType charType)
        {
            string vowels = "аеёиоуыэюя";
            string consonants = "бвгджзйклмнпрстфхцчшщ";
            try {
                switch (charType)
                {
                    case CharType.Consonants:
                        letterStats = letterStats.Where(p => vowels.ToUpper().Contains(p.Letter[0].ToString().ToUpper())).ToList();
                        break;
                    case CharType.Vowel:
                        letterStats = letterStats.Where(p => consonants.ToUpper().Contains(p.Letter[0].ToString().ToUpper())).ToList();
                        break;
                }
            } 
            catch {
                Console.WriteLine("Ошибка при фильтрации статистики " + charType);
                letterStats = new List<LetterStats>();
            }
            
            return letterStats;
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letterStats">Коллекция со статистикой</param>
        private static void PrintStatistic(IList<LetterStats> letterStats)
        {
            try {
                int total = 0;
                letterStats.OrderBy(letterStat => letterStat.Letter[0].ToString().ToUpper()).ToList()
                    .ForEach(value => {
                        Console.WriteLine(value.Letter + " : " + value.Count);
                        total += value.Count;
                    });
                Console.WriteLine("ИТОГО : " + total);
            } 
            catch {
                Console.WriteLine("Ошибка вывода статистики");
            }
            
        }

        /// <summary>
        /// Метод проверяет есть ли в списке выбраный элемент и если нет добавляет его.
        /// </summary>
        /// <param name="letterStats">Коллекция со статистикой</param>
        private static void CheckExistStatistic(IList<LetterStats> letterStats, LetterStats singleLetterStats, string readString)
        {
            string russianLetters = "аеёиоуыэюябвгджзйклмнпрстфхцчшщ";
            if (singleLetterStats == null && russianLetters.ToUpper().Contains(readString[0].ToString().ToUpper()))
            {
                letterStats.Add(new LetterStats() { Count = 1, Letter = readString });
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданному обьекту.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            if (letterStats != null)
            {
                letterStats.Count++;
            }
        }


    }
}
