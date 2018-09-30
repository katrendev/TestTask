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
        private static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            Console.WriteLine("Нажмите любую клавишу, для завершения работы программы:");
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
            stream.ResetPositionToStart();

            //Создаем список структур LetterStats, каждая из которых имеет определенную, нигде не повторяющуюся букву
            var letterStatsList = new List<LetterStats>();

            //Создаем список уже считанных букв для быстроты проверки
            var readLetterList = new List<char>();

            while (!stream.IsEof)
            {
                var currentChar = stream.ReadNextChar();

                // Проверка на то, что текущий символ - буква
                if (!char.IsLetter(currentChar))
                {
                    continue;
                }

                // Проверка на наличие символа в списке уже считанных символов.
                // В случае неуспешной проверки создать новый список буквенной статистики и добавить символ в список считанных символов.
                if (!readLetterList.Contains(currentChar))
                {
                    readLetterList.Add(currentChar);

                    letterStatsList.Add(new LetterStats { Letter = currentChar.ToString(), Count = 1 });
                }
                else
                {
                    foreach (var letterStats in letterStatsList)
                    {
                        if (letterStats.Letter == currentChar.ToString())
                        {
                            IncStatistic(letterStats);
                        }
                    }
                }
            }

            return letterStatsList;
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
            stream.ResetPositionToStart();

            //Создаем список структур LetterStats, каждая из которых имеет определенную, нигде не повторяющуюся букву
            var letterStatsList = new List<LetterStats>();

            //Создаем список уже считанных букв для быстроты проверки
            var readLetterList = new List<string>();

            //Предыдущий символ для поиска пар символов
            char? previousChar = null;

            while (!stream.IsEof)
            {
                //Так как статика не регистрозависимая, то пусть символ будет строчным
                var currentChar = char.ToLower(stream.ReadNextChar());

                // Проверка на то, что текущий символ - буква
                if (!char.IsLetter(currentChar))
                {
                    continue;
                }

                // Проверка на рядомстоящую пару одинаковых символов
                if (currentChar == previousChar)
                {
                    var charPair = previousChar + "" + currentChar;

                    // Проверка на наличие символа в списке уже считанных символов.
                    // В случае неуспешной проверки создать новый список буквенной статистики и добавить символ в список считанных символов.
                    if (!readLetterList.Contains(charPair))
                    {
                        readLetterList.Add(charPair);

                        letterStatsList.Add(new LetterStats { Letter = charPair, Count = 1 });
                    }
                    else
                    {
                        foreach (var letterStats in letterStatsList)
                        {
                            if (letterStats.Letter == charPair)
                            {
                                IncStatistic(letterStats);
                            }
                        }
                    }
                }

                previousChar = currentChar;
            }

            return letterStatsList;
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
            //Список гласных букв для удаления их из статистики
            var vowelsLetters = new List<char> { 'а', 'у', 'о', 'ы', 'и', 'э', 'я', 'ю', 'ё', 'е' };
            //Список согласных букв для удаления их из статистики
            var consonantsList = new List<char> { 'б', 'в', 'г', 'д', 'ж', 'з', 'й', 'к', 'л', 'м', 'н', 'п', 'р', 'с', 'т', 'ф', 'х', 'ц', 'ч', 'ш', 'щ' };

            switch (charType)
            {
                case CharType.Consonants:
                    foreach (var letter in consonantsList)
                    {
                        for (var i = 0; i < letters.Count; i++)
                        {
                            //Проверяем буквы статистики на наличие согласных
                            //Так как удаляем просто согласные, то делаем это независимо от регистра
                            if (!letters[i].Letter.ToLower().Contains(letter.ToString()))
                            {
                                continue;
                            }

                            letters.RemoveAt(i);
                            i = 0;
                        }
                    }
                    break;
                case CharType.Vowel:
                    foreach (var letter in vowelsLetters)
                    {
                        for (var i = 0; i < letters.Count; i++)
                        {
                            //Проверяем буквы статистики на наличие гласных
                            //Так как удаляем просто гласные, то делаем это независимо от регистра
                            if (!letters[i].Letter.ToLower().Contains(letter.ToString()))
                            {
                                continue;
                            }

                            letters.RemoveAt(i);
                            i = 0;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(charType), charType, null);
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="lettersStats">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> lettersStats)
        {
            foreach (var letterStats in lettersStats.OrderBy(l => l.Letter))
            {
                Console.WriteLine(letterStats.Letter + " : " + letterStats.Count);
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