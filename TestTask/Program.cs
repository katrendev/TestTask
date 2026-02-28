using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        const string vowels = "аеёиоуыэюя";
        const string consonants = "бвгджзйклмнпрстфхцчшщъь";

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой СОГЛАСНОЙ буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных ГЛАСНЫХ букв (не регистрозависимо). Например АА, Оо, еЕ, и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь  до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IList<LetterStats> singleLetterStats, doubleLetterStats;

            using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
            { singleLetterStats = FillSingleLetterStats(inputStream1); }
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
            { doubleLetterStats = FillDoubleLetterStats(inputStream2); }

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.Read();
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
            List<LetterStats> fullStat = new List<LetterStats>();
            foreach (char c in alphabet)
            {
                fullStat.Add(new LetterStats { Letter = c.ToString().ToUpper(), Count = 0 });
                fullStat.Add(new LetterStats { Letter = c.ToString(), Count = 0 });
            }
            var fullStat_array = fullStat.ToArray();


            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                int index = Array.FindIndex(fullStat_array, x => x.Letter == c.ToString());
                if (index != -1) IncStatistic(ref fullStat_array[index]);
            }

            var result = from ls in fullStat_array where ls.Count > 0 select ls;
            return result.ToList();
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
            List<LetterStats> fullStat = new List<LetterStats>();
            foreach (char c in alphabet)
            {
                fullStat.Add(new LetterStats { Letter = c.ToString().ToUpper(), Count = 0 });
            }
            var fullStat_array = fullStat.ToArray();


            string previousLetter = "";
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                string readedLetter = c.ToString().ToUpper();
                if (previousLetter == readedLetter)
                {
                    int index = Array.FindIndex(fullStat_array, x => x.Letter == readedLetter);
                    if (index != -1)
                    {
                        IncStatistic(ref fullStat_array[index]);//.Count++;
                        //чтобы не засчитать три одинаковых буквы за две пары
                        previousLetter = "";
                        continue;
                    }
                }
                previousLetter = readedLetter;
            }
            var result = from ls in fullStat_array where ls.Count > 0 select ls;
            return result.ToList();
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
            string excludedLetters = "";
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    excludedLetters = consonants;
                    break;
                case CharType.Vowel:
                    excludedLetters = vowels;
                    break;
            }

            var modifiedLetters = letters.Where(l => !excludedLetters.Contains(l.Letter) &&
                                   !excludedLetters.Contains(l.Letter.ToLower())).ToList();
            letters.Clear();
            foreach (var l in modifiedLetters) letters.Add(l);
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
            foreach (var l in letters) Console.WriteLine($"{l.Letter} : {l.Count}");
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
