using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Program
    {
        private static readonly HashSet<char> _vowels = new HashSet<char>("ЁУЕЫАОЭЯИЮ");

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
            if (args == null|| args.Length <2)
            {
                Console.WriteLine("Нужно указать пути к двум файлам");
                Console.ReadKey(true);
                return;
            }

            try
            {
                using (IReadOnlyStream inputStream1 = GetInputStream(args[0]))
                using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))
                {
                    IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                    IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                    RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                    RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                    PrintStatistic(singleLetterStats);
                    PrintStatistic(doubleLetterStats);
                }

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Ошибка.Файл не найден");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка.{ex.Message}");
            }
            finally
            {
                Console.WriteLine("Нажмите любую клавишу, чтобы выйти");
                Console.ReadKey(true);
            }
           

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
            var statDict=new Dictionary<string,LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c;
                try
                {
                    c = stream.ReadNextChar();
                }
                catch (EndOfStreamException)
                {
                    break;
                }

                if (!char.IsLetter(c))
                {
                    continue;
                }

                string key = c.ToString();
                if (statDict.TryGetValue(key, out var stat))
                {
                    IncStatistic(stat);
                }
                else
                {
                    statDict[key] = new LetterStats { Letter = key, Count = 1 };
                }
            }
                return statDict.Values.ToList();
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
            var statDict=new Dictionary<string,LetterStats>();
            char? prevC = null;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c;
                try
                {
                    c = stream.ReadNextChar();
                }
                catch (EndOfStreamException)
                {
                    break;
                }

                if(!char.IsLetter(c))
                {
                    prevC=null;
                    continue;
                }

                char upperC=char.ToUpper(c);
                if (prevC.HasValue && prevC.Value == upperC)
                {
                    string pair = new string(new char[] { upperC, upperC });
                    if (statDict.TryGetValue(pair, out var stat))
                    {
                        IncStatistic(stat);
                    }
                    else
                    {
                        statDict[pair] = new LetterStats { Letter = pair, Count = 1 };
                    }
                    prevC=null;
                }
                else
                {
                    prevC=upperC;
                }

              
            }

            return statDict.Values.ToList();
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
            for(int i=letters.Count-1; i >= 0; i--)
            {
                var charStat = letters[i];
                if (string.IsNullOrEmpty(charStat.Letter))
                {
                    continue;
                }

                bool isVowel = IsVowel(charStat.Letter[0]   );
                bool isRemove = true;

                switch (charType)
                {
                    case CharType.Consonants:
                        isRemove = !isVowel;
                        break;
                    case CharType.Vowel:
                        isRemove = isVowel;
                        break;
                }

                if (isRemove)
                {
                    letters.RemoveAt(i);
                }

            }


        }

        private static bool IsVowel(char c)
        {
            return _vowels.Contains(char.ToUpper(c));
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
            if (letters == null)
            {
                return;
            }

            var sortLet=letters.OrderBy(x => x.Letter,StringComparer.Ordinal).ToList();
            if (sortLet.Count == 0)
            {
                Console.WriteLine("нет данных");
                return;
            }

            int allCount = 0;
            foreach (var c in sortLet)
            {
                Console.WriteLine($"{c.Letter} : {c.Count}");
                allCount+=c.Count;
            }
            Console.WriteLine($"ИТОГО: {allCount}");
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
