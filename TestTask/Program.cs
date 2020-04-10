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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);//
            

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);

            inputStream1.Close();

            IList<LetterStats> doubleLetterStats = null;
            using (IReadOnlyStream inputStream2 = GetInputStream(args[1]))//)
            {
                doubleLetterStats = FillDoubleLetterStats(inputStream2);
            }

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
            Dictionary<char, LetterStats> ret = new Dictionary<char, LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();

                    if ((c >= 1040 && c <= 1103) || c == 1025 || c == 1105)//проверка на русские буквы
                    {
                        if (!ret.ContainsKey(c))//если ключа нет создаем
                        {
                            ret[c] = new LetterStats() { Letter = c.ToString() };
                        }

                        IncStatistic(ret[c]); ;
                    }
                }
                catch { }

            }

            return ret.Select(kvp => kvp.Value).ToList();

            //throw new NotImplementedException();
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
            Dictionary<string, LetterStats> ret = new Dictionary<string, LetterStats>();
            string previousCharacter = null;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();

                    if (previousCharacter != null)
                    {
                        if (previousCharacter == c.ToString().ToLower() && (c >= 1040 && c <= 1103) || c == 1025 || c == 1105)//проверка на русские буквы и чтобы предыдущяя была такой же
                        {
                            var para = previousCharacter + c.ToString().ToLower();

                            if (!ret.ContainsKey(para))//если ключа нет создаем
                            {
                                ret[para] = new LetterStats() { Letter = para };
                            }
                            IncStatistic(ret[para]);
                        }
                    }

                    previousCharacter = c.ToString().ToLower();
                }
                catch { }
            }

            return ret.Select(kvp => kvp.Value).ToList();

            //throw new NotImplementedException();
        }
        //масив с гласными буквами
        static char[] glas = new char[]
        {
            (char)1040,
            (char)1045,
            (char)1048,
            (char)1054,
            (char)1059,
            (char)1067,
            (char)1069,
            (char)1070,
            (char)1071,
            (char)1072,
            (char)1077,
            (char)1080,
            (char)1086,
            (char)1091,
            (char)1099,
            (char)1101,
            (char)1102,
            (char)1103,
            (char)1025,
            (char)1105
        };

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {

            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = letters.Count - 1; i >= 0; i--)
                    {
                        if (!glas.Contains(letters[i].Letter[0]))
                        {
                            letters.Remove(letters[i]);
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (int i = letters.Count - 1; i >= 0; i--)
                    {
                        if (glas.Contains(letters[i].Letter[0]))
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
            letters=letters.OrderBy(x => x.Letter.ToLower()[0]);
            int itogo = 0;
            foreach (var item in letters)
            {
                Console.WriteLine(item.Letter+ " : "+ item.Count);
                itogo += item.Count;
            }
            Console.WriteLine("ИТОГО : " + itogo);
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
