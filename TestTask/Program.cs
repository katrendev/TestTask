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
            try
            {
                using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                {
                    using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                    {
                        IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                        IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                        PrintStatistic(RemoveCharStatsByType(singleLetterStats, CharType.Vowel));
                        Console.WriteLine("=====================");
                        PrintStatistic(RemoveCharStatsByType(doubleLetterStats, CharType.Consonants));
                    }
                }               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadLine();
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
            IList<LetterStats> result = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char charCurrent = stream.ReadNextChar();

                //если символ не является буквой перейти к следующему
                if (!char.IsLetter(charCurrent))
                {
                    continue;
                }

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                LetterStats row = result.SingleOrDefault(item => item.Letter == charCurrent.ToString());

                if (row != null)
                {
                    IncStatistic(row);
                }
                else
                {
                    result.Add(new LetterStats { Letter = charCurrent.ToString(), Count = 1 });
                }
            }

            return result;
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
            IList<LetterStats> result = new List<LetterStats>();

            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char charCurrent = char.ToUpper(stream.ReadNextChar());

                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                char charNext = char.ToUpper(stream.GetNextChar());

                //если один из символов не является буквой перейти к следующему
                if (!(char.IsLetter(charCurrent) && char.IsLetter(charNext)))
                {
                    continue;
                }

                if (charCurrent == charNext)
                {
                    LetterStats row = result.SingleOrDefault(item => item.Letter == $"{charCurrent}{charNext}");

                    if (row != null)
                    {
                        IncStatistic(row);
                    }
                    else
                    {
                        result.Add(new LetterStats { Letter = $"{charCurrent}{charNext}", Count = 1 });
                    }
                }

            }

            return result;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Возвращает отфильтрованную от иных символов коллекцию 
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            //switch (charType)
            //{
            //    case CharType.Consonants:
            //        break;
            //    case CharType.Vowel:
            //        break;
            //}

            return letters.Where(item => item.Letter.Count() > 0 && item.Letter[0].GetCharType() == charType).ToList();
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            foreach (var letter in letters.OrderBy(item => item.Letter))
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
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