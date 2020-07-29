using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

            RemoveCharStatsByType(ref singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(ref doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.ReadKey(); 
        }
        
        /// <summary>
        /// Экземпляр класса HashSet с массивом гласных букв для последующего определения параметра 
        /// CharType в статистике, игнорируя регистр
        /// </summary>
        private static HashSet<string> Vowel = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "а", "о", "у", "е", "ы", "я", "и", "ё", "ю", "a", "e", "i", "o", "u", "y" };

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
        /// Функция обновляет статистику при найденном совпадении считанного символа с элементом этой статистики
        /// </summary>
        /// <param name="list">коллекция с буквами и из количеством</param>
        /// <param name="index">индекс инкрементируемого значения</param>
         private static void OnFoundIndex(IList<LetterStats> list, int index)
        {
            var foundEntity = list[index];
            IncStatistic(ref foundEntity);
            list[index] = foundEntity;
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
            List<LetterStats> singleLetters = new List<LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                string value = c.ToString();
                if (!Char.IsLetter(c)) // если символ не является буквой - идем на следующую итерацию
                    continue;

                var foundIndex = singleLetters.FindIndex(stat => stat.Letter.Equals(value));
                if (foundIndex >= 0)
                {
                    OnFoundIndex(singleLetters, foundIndex);
                }
                else // если в коллекции записи о букве нет - создаем запись, определяя тип звука
                {
                    CharType type = Vowel.Contains(value) ? CharType.Vowel : CharType.Consonants;
                    singleLetters.Add(new LetterStats(type, value, 1));
                }  
            }
            return singleLetters;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// UPD: на эквивалетность символы проверяются по типу "скользящего окна", поэтому комбинацию "АаА"
        /// программа засчитает, причем как две пары: "Аа"А и А"аА".
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            stream.ResetPositionToStart();
            List<LetterStats> doubleLetters = new List<LetterStats>();

            string pattern = @"\w+";
            Regex rgx = new Regex(pattern);
            string value = string.Empty;
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                value += c;
                if (value.Length < 2) // проверяет только парами
                    continue;
                if (value.Length == 3)
                    value = value.Remove(0, 1); 
                if (!rgx.IsMatch(value))
                    continue;
                var a = value[0].ToString();
                var b = value[1].ToString();
                if (!String.Equals(a, b, StringComparison.OrdinalIgnoreCase)) // игнорирует пары неэквивалентных символов (регистр игнорируется)
                    continue;
                var foundIndex = doubleLetters.FindIndex(stat => stat.Letter.Equals(value, StringComparison.OrdinalIgnoreCase));

                if (foundIndex >= 0)
                {
                    OnFoundIndex(doubleLetters, foundIndex);
                }
                else // если в коллекции записи о букве нет - создаем запись, определяя тип звука. 
                {
                    CharType type = Vowel.Contains(a) ? CharType.Vowel : CharType.Consonants;
                    doubleLetters.Add(new LetterStats(type, value.ToUpper(), 1)); // Для эстетичности вывода регистр меняется на заглавные
                }

            }
            return doubleLetters;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(ref IList<LetterStats> letters, CharType charType)
        {
            var ls = letters.Where(l => l.charType == charType);
            letters = letters.Except(ls).ToList();
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
            int counter = 0;
            var sortedLetters = from l in letters
                                orderby l.Letter
                                select l;
            Console.WriteLine("----------");
            foreach (LetterStats ls in sortedLetters)
            {
                Console.WriteLine($"{ls}");
                counter += ls.Count;
            }
            Console.WriteLine("ИТОГО: " + counter);
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
