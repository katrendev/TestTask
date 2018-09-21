using System;
using System.Collections.Generic;
using System.IO;
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
            // Проверим, что указаны оба файла для чтения
            if (args.Length != 2)
            {
                PrintAndClose("В параметрах запуска должны быть указаны два пути до файлов");
            }

            SetConsoleEncoding(System.Text.Encoding.Unicode);

            try
            {
                // Делать через FileInfo совсем не обязательно, но в методе GetInputStream написано, что указывается "Полный путь до файла для чтения"
                // Плюс, можно будет вывести в консоль более понятный полный путь до файла, а не относительный, если его вдруг укажут в таком виде.
                var fileInfo1 = new FileInfo(args[0]);
                var fileInfo2 = new FileInfo(args[1]);
                IList<LetterStats> singleLetterStats;
                IList<LetterStats> doubleLetterStats;

                // Считаем данные и сразу освободим ресурсы
                using (IReadOnlyStream inputStream1 = GetInputStream(fileInfo1.FullName),
                                       inputStream2 = GetInputStream(fileInfo2.FullName))
                {
                        singleLetterStats = FillSingleLetterStats(inputStream1);
                        doubleLetterStats = FillDoubleLetterStats(inputStream2);
                }

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel, true);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants, true);
                Console.WriteLine();

                Console.WriteLine($"Статистика для файла {fileInfo1.FullName}:");
                PrintStatistic(singleLetterStats);
                Console.WriteLine();
                Console.WriteLine($"Статистика для файла {fileInfo2.FullName}:");
                PrintStatistic(doubleLetterStats);
            }
            catch (Exception e)
            {
                PrintAndClose(e.Message);
            }
            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            PrintAndClose("");
        }

        /// <summary>
        /// Устанавливает для вывода консоли указанную кодировку.
        /// Для того, чтобы можно было нормально увидеть нестандартные буквы(например, японские), нужно будет в настройках консоли выбрать подходящий шрифт.
        /// </summary>
        /// <param name="encoding">Кодировка</param>
        private static void SetConsoleEncoding(Encoding encoding)
        {
            // Переведем консоль в режим отображения Unicode, если получится.
            // Для того, чтобы можно было нормально увидеть нестандартные буквы(например, японские), нужно будет в настройках консоли выбрать подходящий шрифт.
            try
            {
                System.Console.OutputEncoding = encoding;
            }
            catch (Exception)
            {
                Console.WriteLine($"Не удалось перевести консоль в режим отображения символов Unicode.\nБудет использоваться текущая кодовая страница: {Console.OutputEncoding.CodePage}");
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
           List<LetterStats> dictionary = new List<LetterStats>();
            stream.ResetPositionToStart();

            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                    if (char.IsLetter(c))
                    {
                        var entry = dictionary.Where(e => e.Letter.Equals(c.ToString(),
                                     StringComparison.InvariantCulture)).SingleOrDefault();
                        if (entry == null) //Если не нашли - добавим
                        {
                            dictionary.Add(new LetterStats(c.ToString()));// Т.к. регистрозависимая статистика - добавляем ключи как есть.
                        }
                        else
                        {
                            IncStatistic(entry);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            return dictionary;
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
            List<LetterStats> dictionary = new List<LetterStats>();
            char? prevChar = null;
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                try
                {
                    char c = stream.ReadNextChar();
                    if (!prevChar.HasValue)
                    {
                        prevChar = c; //запоминаем
                        continue;
                    }
                    // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                    // ??? Не регистрозависимые пары букв, судя по описанию программы. Если делать тут регистрозависимый поиск - то статистика для с и С будет одинаковой...
                    if (char.IsLetter(c) && prevChar.ToString().Equals(c.ToString(),
                                     StringComparison.InvariantCultureIgnoreCase)) //Если это символ и он равен предыдущему(без учета регистра) - добавляем в словарь.
                    {
                        var entry = dictionary.Where(e => e.Letter[0].ToString().Equals(c.ToString(),
                                     StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
                        if (entry == null) //Если не нашли - добавим
                        {
                            dictionary.Add(new LetterStats((c.ToString() + c.ToString()).ToLower()));// Т.к. регистронезависимая статистика - добавляем ключи в нижнем регистре.
                        }
                        else
                        {
                            IncStatistic(entry);
                        }
                    }
                    prevChar = c; //запоминаем
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        /// <param name="printRemoveInfo">Вывести в консоль информацию об удаленных данных</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType, bool printRemoveInfo)
        {
            int removedCount = 0;
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    removedCount = letters.RemoveAll(i => GetType(i.Letter[0]) == CharType.Consonants);
                    break;
                case CharType.Vowel:
                    removedCount = letters.RemoveAll(i => GetType(i.Letter[0]) == CharType.Vowel);
                    break;
            }
            if (printRemoveInfo)
            {
                Console.WriteLine($"Из статистики удалены {GetEnumDescription(charType)}, {removedCount} записей");
            }
        }
        
        /// <summary>
        /// Определяет тип переданного символа, гласная или согласная
        /// Поддерживается кирилилца и латиница. Для всех остальных буквы будут считаться согласными.
        /// </summary>
        /// <param name="c">Символ у которого нужно определить тип</param>
        /// <returns></returns>
        public static CharType GetType(char c)
        {
            return "aeiouУЕЫАОЭЯИЮ".IndexOf(c.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0 ? CharType.Vowel : CharType.Consonants;
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
            int totalMathesCount = 0;
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            foreach (var entry in letters.OrderBy(e=>e.Letter))
            {
                totalMathesCount += entry.Count;
                Console.WriteLine($"{entry.Letter} : {entry.Count}");
            }
            Console.WriteLine($"Итого найдено различных букв/пар: {letters.Count()}; всего совпадений: {totalMathesCount}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.IncCount();
        }

        /// <summary>
        /// Выводит сообщение в консоль, после чего ждет нажатия любой клавиши и завершает работу программы
        /// </summary>
        /// <param name="Message"></param>
        private static void PrintAndClose(string Message)
        {
            Console.WriteLine(Message);
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static string GetEnumDescription(Enum value)
        {
            System.Reflection.FieldInfo fi = value.GetType().GetField(value.ToString());

            System.ComponentModel.DescriptionAttribute[] attributes =
                (System.ComponentModel.DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(System.ComponentModel.DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
