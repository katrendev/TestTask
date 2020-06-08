using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
#if DEBUG
            args = new[] { "C:\\Work\\Repositari\\TestTask\\1.txt",  "C:\\Work\\Repositari\\TestTask\\2.txt"};
            args = new[] { "d:\\Work\\111\\TestTask\\1.txt", "d:\\Work\\111\\TestTask\\2.txt" };
#endif

            if (args.Length == 0)
            {
                Console.WriteLine("Нет имен файлов");
                string n = null;
                while (n == null)
                {
                    n = Console.ReadLine();
                }
                return;
            }
            char[] _dataFile1;      //считанные данные файла 1
            char[] _dataFile2;      //считанные данные файла 2


            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            _dataFile1 = inputStream1.ReadFile();
            int l = _dataFile1.Length;
            IList<LetterStats> singleLetterStats = FillSingleLetterStats(_dataFile1);
            IList<LetterStats> singleLetterStatsRes = RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            PrintStatistic(singleLetterStatsRes);

            if (args.Length >= 2)
            {
                IReadOnlyStream inputStream2 = GetInputStream(args[1]);
                _dataFile2 = inputStream2.ReadFile();
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(_dataFile2);
                IList<LetterStats> doubleLetterStatsres = RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(doubleLetterStatsres);
            }

            Console.WriteLine("Для завершения работы Enter");
            string nn = null;
            while (nn == null)
            {
                nn = Console.ReadLine();
            }
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
        private static IList<LetterStats> FillSingleLetterStats(char[] data)
        {
            IList<LetterStats>pList = new List<LetterStats>();


            foreach (var t in data)
            {
                string tmp = t.ToString();
                if (tmp != " " && tmp != ".")
                {
                    var varible = pList.Where(q => q.Letter.Contains(tmp));

                    IEnumerator<LetterStats> pList1 = varible.GetEnumerator();
                    if (varible.LongCount() == 0)
                    {
                        LetterStats letterStats = new LetterStats();
                        letterStats.Letter = tmp;
                        letterStats.Count = 0;
                        IncStatistic(ref letterStats);
                        pList.Add(letterStats);
                    }
                    else
                    {
                        bool res = pList1.MoveNext();
                        if (res)
                        {
                            int index = pList.IndexOf(pList1.Current);
                            LetterStats letterStats = pList[index];
                            IncStatistic(ref letterStats);
                            pList[index] = letterStats;
                        }
                    }
                }
            }
            /* pList.
             stream.ResetPositionToStart();
             while (!stream.IsEof)
             {
                 char c = stream.ReadNextChar();
                 // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
             }*/

            return pList;           
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(char [] data)
        {
            IList<LetterStats>pList = new List<LetterStats>();
            for (int i=0; i<data.Length; i++)
            {
                string tmp = data[i].ToString();
                if (tmp != " " && tmp != ".")
                {
                    string tmp1 = tmp.ToUpper();
                    if (i < data.Length - 1)
                    {
                        if (tmp1 == data[i + 1].ToString().ToUpper())
                        {
                            i++;
                            tmp1 += tmp1;
                            var varible = pList.Where(q => q.Letter.Contains(tmp1));

                            IEnumerator<LetterStats> pList1 = varible.GetEnumerator();
                            if (varible.LongCount() == 0)
                            {
                                LetterStats letterStats = new LetterStats();
                                letterStats.Letter = tmp1;
                                letterStats.Count = 0;
                                IncStatistic(ref letterStats);
                                pList.Add(letterStats);
                            }
                            else
                            {
                                bool res = pList1.MoveNext();
                                if (res)
                                {
                                    int index = pList.IndexOf(pList1.Current);
                                    LetterStats letterStats = pList[index];
                                    IncStatistic(ref letterStats);
                                    pList[index] = letterStats;
                                }
                            }
                        }
                    }
                }
            }

            return pList;           
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static IList<LetterStats> RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            IList<LetterStats> pList = new List<LetterStats>();
            char[] _VowelLater = { 'а', 'у', 'о', 'ы', 'и', 'э', 'я', 'ю', 'ё', 'е' };
            char[] _ConsonantsLater = { 'б', 'в', 'г', 'д', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = 0; i < letters.Count; i++)
                    {
                        bool res = false;
                        for (int j = 0; j < _ConsonantsLater.Length; j++)
                        {
                            if (letters[i].Letter.Substring(0, 1) == _ConsonantsLater[j].ToString())
                            {
                                res = true;
                                break;
                            }
                            else if (letters[i].Letter.Substring(0, 1) == _ConsonantsLater[j].ToString().ToUpper())
                            {
                                res = true;
                                break;
                            }
                        }
                        if(!res)
                            pList.Add(letters[i]);
                    }

                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.Count; i++)
                    {
                        bool res = false;
                        for (int j = 0; j < _VowelLater.Length; j++)
                        {
                            if (letters[i].Letter.Substring(0, 1) == _VowelLater[j].ToString())
                            {
                                res = true;
                                break;
                            }

                            if (letters[i].Letter.Substring(0, 1) == _VowelLater[j].ToString().ToUpper())
                            {
                                res = true;
                                break;
                            }
                        }
                        if (!res)
                            pList.Add(letters[i]);
                    }
                    break;
            }

            return pList;
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

            LetterStats pLetterStats;

            var letterStatses = letters as LetterStats[] ?? letters.ToArray();
            for (int i = 0; i < letterStatses.Count() - 1; i++)
            {
                for (int j = i + 1; j < letterStatses.Count() - 1; j++)
                {
                    char bt1 = Convert.ToChar(letterStatses[i].Letter.Substring(0, 1).ToUpper());
                    char bt2 = Convert.ToChar(letterStatses[j].Letter.Substring(0, 1).ToUpper());
                    if (bt1 >bt2)
                    {
                        pLetterStats = letterStatses[i];
                        letterStatses[i] = letterStatses[j];
                        letterStatses[j] = pLetterStats;
                    }
                }
            }

            int count = 0;
            for (int i = 0; i < letterStatses.Count() ; i++)
            {
                Console.WriteLine(letterStatses[i].Letter + " : " + letterStatses[i].Count);
                count += letterStatses[i].Count;
            }
            Console.WriteLine("ИТОГО: " + count);
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
