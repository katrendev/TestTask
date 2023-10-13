using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
            IReadOnlyStream inputStream1 = null, inputStream2 = null;
            try
            {
                inputStream1 = GetInputStream(args[0]);
                inputStream2 = GetInputStream(args[1]);


                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                inputStream1?.Dispose();
                inputStream2?.Dispose();
            }
            // Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
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
        /// Ф-ция возвращает тип буквы (согласная или гласная), если это не буква, возвращает NaL
        /// </summary>
        /// <param name="c">Буква</param>
        /// <returns>Тип буквы</returns>
        private static CharType GetCharType(char c)
        {
            if (!char.IsLetter(c))
                return CharType.NaL;
            const string VowelChars = "aeiouyоайуеэяиюы";
            c = char.ToLower(c);
            if (VowelChars.Any(x=> x == c))
                return CharType.Vowel;
            else
                return CharType.Consonants;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
            => FillSequenceLetterStats(stream, 1, true);

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа.</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream) 
            => FillSequenceLetterStats(stream, 2, false, CharSize.Upper);

        /// <summary>
        /// Ф-ция добавляет в результирующую коллекцию все последовательности из одинаковых символов определенной длины.
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа.</param>
        /// <param name="lengthSequence">Количество одинаковых букв, которые должны идти подряд.</param>
        /// <param name="caseSensitive">Анализ регистрозависим, или нет.</param>
        /// <param name="size">Регистр результирующего списка.</param>
        /// <returns>Коллекция статистик по каждой букве (или последовательности одинаковых букв), что была прочитана из стрима.</returns>
        /// <exception cref="ArgumentException"></exception>
        private static IList<LetterStats> FillSequenceLetterStats(IReadOnlyStream stream, int lengthSequence, bool caseSensitive, CharSize? size = null)
        {
            if ( caseSensitive == true && size != null)
            {
                throw new ArgumentException("Если последовательность регистрозависима, то нельзя определить регистр результирующего списка.");
            }
            if (caseSensitive == false && size == null)
            {
                throw new ArgumentException("Если последовательность нерегистрозависима, то необходимо определить регистр результирующего списка.");
            }
            stream.ResetPositionToStart();
            List<LetterStats> list = new List<LetterStats>();
            LettersSequence sequence = new LettersSequence(lengthSequence);
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!caseSensitive)
                {
                    if (size == CharSize.Lower)
                    {
                        c = char.ToLower(c);
                    }
                    else if (size == CharSize.Upper)
                    {
                        c = char.ToUpper(c);
                    }
                }
                    
                CharType type = GetCharType(c);
                if (type == CharType.NaL)
                {
                    sequence.Clear();
                    continue;
                }
                sequence.Add(c);
                char? ans;
                if ((ans = sequence.IsAllSame()) != null)
                {
                    int index = list.FindIndex(x => x.Letter[0] == ans);
                    if (index < 0)
                    {
                        list.Add(new LetterStats(string.Concat(sequence.Letters), type, 1));
                    } 
                    else
                    {
                        IncStatistic(list[index]);
                    }
                    sequence.Clear();
                }
            }
            return list;
        }


        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType) 
            => letters.RemoveAll(x => x.Type == charType);


        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            foreach (var item in letters.OrderBy(x => x.Letter))
            {
                Console.WriteLine($"{item.Letter} : {item.Count}");
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats) 
            => letterStats.Count++;


    }
}
