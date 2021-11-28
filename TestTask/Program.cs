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
            //на время разработки: 
            args = new string[2];
            args[0] = @"file1.txt";
            args[1] = @"file2.txt";
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            List<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            List<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);


            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);



            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Нажмите любую клавишу чтобы закрыть приложение.");
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
        private static List<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letters = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                letters.Add(new LetterStats() { Letter = c.ToString() });
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.

                IncStatistic(letters, true);
            }
            return letters;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static List<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letters = new List<LetterStats>();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                string c = stream.ReadNextChar().ToString().ToUpper();
                if (stream.IsEof == false && stream.ReadNextChar().ToString().ToUpper() == c)
                {

                    letters.Add(new LetterStats() { Letter = c + c });

                    IncStatistic(letters, false);
                }
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
            }
            return letters;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(List<LetterStats> letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            string consonants = "Б,б, В,в, Г,г, Д,д, Ж,ж, З,з, Й,й, К,к, Л,л, М,м, Н,н, П,п, Р,р, С,с, Т,т, Ф,ф, Х,х, Ц,ц, Ч,ч, Ш,ш, Щ,щ";
            string vowel = "А, а, У,у, О,о, Ы,ы, И,и, Э,э, Я,я, Ю,ю, Ё,ё, Е,е";
            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = 0; i < letters.Count - 1; i++)
                    {
                        for (int j = 0; j < consonants.Length; j++)
                        {
                            if (letters[i].Letter.Contains(consonants[j].ToString()))
                            {
                                letters.Remove(letters[i]);
                            }
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.Count; i++)
                    {
                        for (int j = 0; j < vowel.Length; j++)
                        {
                            if (letters[i].Letter.Contains(vowel[j].ToString()))
                            {
                                letters.Remove(letters[i]);
                            }
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
        private static void PrintStatistic(List<LetterStats> letters)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            letters.OrderBy(LetterStats => LetterStats.Letter);
            for (int i = 0; i < letters.Count; i++)
            {
                if (letters[i].Letter == '~'.ToString())
                {
                    letters.Remove(letters[i]);
                }
            }
            foreach (var item in letters)
            {

                Console.WriteLine(item.Letter + " : " + item.Count);
            }
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(List<LetterStats> letters, bool register)
        {
            LetterStats temp = letters[letters.Count - 1];
            if (register == false)
            {
                for (int i = 0; i < letters.Count; i++)
                {
                    if (letters[i].Letter.ToUpper() == letters[letters.Count - 1].Letter.ToUpper())
                    {
                        temp.Count++;

                    }
                }
            }
            else
            {
                for (int i = 0; i < letters.Count; i++)
                {

                    if (letters[i].Letter == letters[letters.Count - 1].Letter)
                    {
                        temp.Count++;
                    }
                }
            }
            letters[letters.Count - 1] = temp;
        }

    }
}
