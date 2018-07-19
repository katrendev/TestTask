using System;
using System.Collections.Generic;

namespace TestTask
{
    public class Program
    {
        static string VowelLetter = "аеёиоуыэюя";
        static string ConsonantLetter = "бвгджзйклмнпрстфхцчшщъь";

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
            AlphabetLetter alphabetSingleLetter = new AlphabetLetter("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");
            AlphabetLetter alphabetDoubleLetter = new AlphabetLetter("абвгдеёжзийклмнопрстуфхцчшщъыьэюя");

            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            //IList<LetterStats> singleLetterStats = 
            FillSingleLetterStats(inputStream1, alphabetSingleLetter);
            PrintStatisticSingle(alphabetSingleLetter);
            

            //IList<LetterStats> doubleLetterStats = 
            FillDoubleLetterStats(inputStream2, alphabetDoubleLetter);
            PrintStatisticDouble(alphabetDoubleLetter);
            
            Console.WriteLine("Убираем гласные.");
            RemoveCharStatsByType(alphabetSingleLetter, CharType.Vowel);
            PrintStatisticSingle(alphabetSingleLetter);

            Console.WriteLine("Убираем согласные.");
            RemoveCharStatsByType(alphabetDoubleLetter, CharType.Consonants);
            PrintStatisticDouble(alphabetDoubleLetter);

            inputStream1.Close();
            inputStream2.Close();

            // TODO : Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.Read();
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
        //private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream, AlphabetSingleLetter alphabetSingleLetter)
        private static void FillSingleLetterStats(IReadOnlyStream stream, AlphabetLetter alphabetSingleLetter)
        {
            //stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                //denny7794: метод IncStatistic не используется.
                for (int i = 0; i < alphabetSingleLetter.alphabet.Length; i++)
                {
                    if(c == alphabetSingleLetter.alphabet[i])
                    {
                        alphabetSingleLetter.letterCounter[i]++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        //private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        private static void FillDoubleLetterStats(IReadOnlyStream stream, AlphabetLetter alphabetDoubleLetter)
        {
            char c1='0', c2;
            stream.ResetPositionToStart();
            if (!stream.IsEof)
            {
                c1 = Char.ToLower(stream.ReadNextChar());
            }

            while (!stream.IsEof)
            {
                c2 = Char.ToLower(stream.ReadNextChar());
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                for (int i = 0; i < alphabetDoubleLetter.alphabet.Length; i++)
                {
                    if (c1 == alphabetDoubleLetter.alphabet[i] && c2 == alphabetDoubleLetter.alphabet[i])
                    {
                        alphabetDoubleLetter.letterCounter[i]++;
                        break;
                    }
                }
                c1 = c2;
            }
        }

        

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        //private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        private static void RemoveCharStatsByType(AlphabetLetter letters, CharType charType)
        {
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    for (int i = 0; i < letters.alphabet.Length; i++)
                    {
                        for (int j = 0; j < ConsonantLetter.Length; j++)
                        {
                            if (Char.ToLower(letters.alphabet[i]) == ConsonantLetter[j])
                            {
                                letters.letterCounter[i] = 0;
                            }
                        }
                    }
                    break;
                case CharType.Vowel:
                    for (int i = 0; i < letters.alphabet.Length; i++)
                    {
                        for (int j = 0; j < VowelLetter.Length; j++)
                        {
                            if (Char.ToLower(letters.alphabet[i]) == VowelLetter[j])
                            {
                                letters.letterCounter[i] = 0;
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
        //private static void PrintStatistic(IEnumerable<LetterStats> letters)
        private static void PrintStatisticSingle(AlphabetLetter alphabetSingleLetter)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            //throw new NotImplementedException();
            int sum = 0;
            for (int i = 0; i < alphabetSingleLetter.alphabet.Length; i++)
            {
                Console.WriteLine("'{0}' : {1}", alphabetSingleLetter.alphabet[i], alphabetSingleLetter.letterCounter[i]);
                if (alphabetSingleLetter.letterCounter[i] != 0)
                {
                    sum ++;
                }
            }
            Console.WriteLine("Общее кол-во найденных букв: {0}\n", sum);
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        //private static void PrintStatistic(IEnumerable<LetterStats> letters)
        private static void PrintStatisticDouble(AlphabetLetter alphabetDoubleLetter)
        {
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            //throw new NotImplementedException();
            int sum = 0;
            for (int i = 0; i < alphabetDoubleLetter.alphabet.Length; i++)
            {
                Console.WriteLine("'{0}{0}' : {1}", alphabetDoubleLetter.alphabet[i], alphabetDoubleLetter.letterCounter[i]);
                if (alphabetDoubleLetter.letterCounter[i] != 0)
                {
                    sum++;
                }
            }
            Console.WriteLine("Общее кол-во найденных пар букв: {0}\n", sum);
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
