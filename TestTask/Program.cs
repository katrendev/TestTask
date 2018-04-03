using System;
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
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            CloseStream(inputStream1);
            CloseStream(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Press any key to exit.");
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


        private static void CloseStream(IReadOnlyStream str)
        {
            str.CloseInputStream();
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        {
            bool alrdHave = false;
            IList<LetterStats> letters = new List<LetterStats>();
            LetterStats ls = new LetterStats();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                byte[] b = new byte[1];
                b[0] = (byte)c;
                bool check1 = (c >= 97 && c <= 122) || (c >= 65 && c <= 90);
                bool check2 = (c >= 224 && c <= 255) || (c >= 192 && c <= 223);
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                if (check1 || check2)
                {
                    ls.Letter = Encoding.GetEncoding("windows-1251").GetString(b);//.ToString();
                    int _index = 0;
                    foreach (LetterStats _ls in letters)
                    {
                        if (_ls.Letter == ls.Letter)
                        {
                            alrdHave = true;
                            break;
                        }
                        else
                        {
                            alrdHave = false;
                        }
                        _index++;
                    }
                    if (!alrdHave)
                    {
                        if (AnalyzeLetter(ls.Letter) == 1)
                        {
                            ls.CharType = CharType.Vowel;
                        }
                        else
                        {
                            ls.CharType = CharType.Consonants;
                        }
                        letters.Add(ls);
                    }
                    LetterStats letter = letters[_index];
                    IncStatistic(ref letter);
                    letters[_index] = letter;
                }
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
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            bool alrdHave = false;
            IList<LetterStats> letters = new List<LetterStats>();
            LetterStats ls = new LetterStats();
            StringBuilder s = new StringBuilder();
            stream.ResetPositionToStart();
            while (!stream.IsEof)
            {
                char c1 = stream.ReadNextChar();
                char c2 = stream.ReadNextChar();

                byte[] b1 = new byte[1];
                byte[] b2 = new byte[1];

                b1[0] = (byte)c1;
                b2[0] = (byte)c2;

                bool check1 = (c1 >= 97 && c1 <= 122) || (c1 >= 65 && c1 <= 90);
                bool check2 = (c1 >= 224 && c1 <= 255) || (c1 >= 192 && c1 <= 223);
                if (check1 || check2)
                {
                    bool isOk = Math.Abs(c1 - c2) == 32 || Math.Abs(c1 - c2) == 0;
                    if (isOk)
                    {
                        s.Clear();
                        s.Append(Encoding.GetEncoding("windows-1251").GetString(b1)).Append(Encoding.GetEncoding("windows-1251").GetString(b2));
                        ls.Letter = s.ToString();
                        int _index = 0;
                        foreach (LetterStats _ls in letters)
                        {
                            if (_ls.Letter == ls.Letter)
                            {
                                alrdHave = true;
                                break;
                            }
                            else
                            {
                                alrdHave = false;
                            }
                            _index++;
                        }
                        if (!alrdHave)
                        {
                            if (AnalyzeLetter(ls.Letter) == 1)
                            {
                                ls.CharType = CharType.Vowel;
                            }
                            else
                            {
                                ls.CharType = CharType.Consonants;
                            }
                            letters.Add(ls);
                        }
                        LetterStats letter = letters[_index];
                        IncStatistic(ref letter);
                        letters[_index] = letter;
                    }
                    else
                    {
                        stream.ResetPosBack();
                    }
                }
                else
                {
                    stream.ResetPosBack();
                }
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
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            for (int i = 0; i < letters.Count; i++)
            {
                if (letters[i].CharType == charType)
                {
                    letters.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту. (orderby)
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IList<LetterStats> letters)
        {
            // сортировка
            letters = letters.OrderBy(obj => obj.Letter).ToList<LetterStats>();

            // вывод на экран
            foreach (LetterStats ls in letters)
            {
                Console.WriteLine(ls.Letter + " : " + ls.Count);
            }
            Console.WriteLine(Environment.NewLine);
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats)
        {
            letterStats.Count++;
        }

        /// <summary>
        /// Функция определяет, является буква гласной или согласной
        /// </summary>
        /// <param name="str"> Буква/Пара букв </param>
        /// <returns> 1 - гласная, 0 - согласная </returns>
        private static int AnalyzeLetter(string str)
        {
            string voxel_lat = "aeiouy";
            string voxel_kir = "аеёиоуыэюя";

            string voxels = voxel_lat + voxel_kir;
            char[] arr_voxels = voxels.ToCharArray();

            str = str.ToLower();
            char[] ch = str.ToCharArray();

            foreach (char c in arr_voxels)
            {
                if (ch[0] == c)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
