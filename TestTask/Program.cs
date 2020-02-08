using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTask
{
    public class Program
    {
        private static char[] Vowels = new char[] { 'а', 'у', 'о', 'ы', 'и', 'э', 'я', 'ю', 'ё', 'е', 'a', 'e', 'i', 'o', 'u',  };
        private static char[] Consonants = new char[] { 'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т',
            'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'b', 'c', 'd', 'f', 'g', 'h', 'j', 
            'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };



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
            Console.OutputEncoding = Encoding.UTF8;
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
            IList<LetterStats> letterStateList = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                    continue;

                string letter = new String(c, 1);
                if (letter == " ")
                    continue;

                var letterState = letterStateList.FirstOrDefault(o => o.Letter == letter);
                if (letterState == null)
                {
                    letterState = new LetterStats(letter);

                    letterStateList.Add(letterState);
                }

                IncStatistic(letterState);
            }

            return letterStateList;

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
            string oldLetter = "";
            IList<LetterStats> letterStateList = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                if (!Char.IsLetter(c))
                    continue;

                string letter = new String(c, 1);
                if (letter == " ")
                    continue;

                if (oldLetter.ToUpper() != letter.ToUpper())
                {
                    oldLetter = letter;
                    continue;
                }

                var letterState = letterStateList.FirstOrDefault(o => o.Letter.ToUpper() == letter.ToUpper() + oldLetter.ToUpper());
                if (letterState == null)
                {
                    letterState = new LetterStats(letter + oldLetter);
                    letterStateList.Add(letterState);
                }
                IncStatistic(letterState);

                oldLetter = "";
            }

            return letterStateList;
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
            for (int i = letters.Count - 1; i >= 0; i--)
            {
                switch (charType)
                {
                    case CharType.Consonants:
                        if (Consonants.Contains(letters[i].Letter.ToLower()[0]))
                            letters.RemoveAt(i);
                        break;
                    case CharType.Vowel:
                        if (Vowels.Contains(letters[i].Letter.ToLower()[0]))
                            letters.RemoveAt(i);
                        break;
                }
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
            int count = 0;
            foreach (var item in letters.OrderBy(o => o.Letter))
            {
                count += item.Count;
                Console.WriteLine($"{item.Letter} : {item.Count}");
            }
            Console.WriteLine($"ИТОГО: {count}");
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
