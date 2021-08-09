using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestTask {
    public class Program {

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args) {
            // проверяем наличие аргументов
            if (args.Length < 2) {
                Console.WriteLine("Требуется два параметра");
                return;
            }
            // ловим исключения т.к. работаеи с потоками
            try {
                IReadOnlyStream inputStream1 = GetInputStream(args[0]);
                IReadOnlyStream inputStream2 = GetInputStream(args[1]);

                IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
                IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

                RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
                RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

                PrintStatistic(singleLetterStats);
                PrintStatistic(doubleLetterStats);
            } catch (Exception ex) {
                Console.WriteLine($"Во время исполнения программы возникла ошибка {ex.Message}");
            }
            Console.WriteLine("Нажмите кнопку для выхода");
            //ждём нажатия клавиши
            Console.ReadKey(true);
            
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath) {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream) {
            //  список найденных букв
            IList<LetterStats> list = new List<LetterStats>();
            // ловим исключения т.к. работаем с потоками
            try {
                stream.ResetPositionToStart();
                while (!stream.IsEof) {
                    char c = stream.ReadNextChar();
                    // обрабатываем только буквы
                    if (char.IsLetter(c)) {
                        //  ищем очередную букву в списке
                        IEnumerable<LetterStats> lsc = list.Where(a => a.Letter == c.ToString());
                        //  временная переменная
                        LetterStats ls;
                        if (lsc.Count() > 0) {
                            //  если нашли букву в списке. записываем во временную переменную
                            ls = lsc.First();
                        } else {
                            //  если не нашли добавляем в список
                            ls = new LetterStats();
                            ls.Letter = c.ToString();
                            list.Add(ls);
                        }
                        //  увеличиваем  статистику
                        IncStatistic(ref ls);
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception in FillSingleLetterStats: {ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream) {
            //  список найденных букв
            IList<LetterStats> list = new List<LetterStats>();
            // ловим исключения т.к. работаем с потоками
            try {
                stream.ResetPositionToStart();
                //  считываем букву во временную переменную
                char p = stream.ReadNextChar();
                while (!stream.IsEof) {
                    if (char.IsLetter(p))
                        break;
                    p = stream.ReadNextChar();
                }
                while (!stream.IsEof) {
                    char c = stream.ReadNextChar();
                    // обрабатываем только буквы
                    if (char.IsLetter(c)) {
                        //  если очередная буква и предыдущая равны
                        if (c.ToString().ToLower() == p.ToString().ToLower()) {
                            //  делаем пару
                            string s = p.ToString() + c.ToString();
                            //  ищем очередную пару в списке
                            IEnumerable<LetterStats> lsc = list.Where(a => a.Letter == s);
                            //  временная переменная
                            LetterStats ls;
                            if (lsc.Count() > 0) {
                                //  если нашли пару в списке. записываем во временную переменную
                                ls = lsc.First();
                            } else {
                                //  если не нашли добавляем в список
                                ls = new LetterStats();
                                ls.Letter = s;
                                list.Add(ls);
                            }
                            //  увеличиваем  статистику
                            IncStatistic(ref ls);
                        }
                        //  сохраняем букву
                        p = c;
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine($"Exception in FillDoubleLetterStats: { ex.Message}");
            }
            return list;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType( IList<LetterStats> letters, CharType charType) {
            if (!letters.Any()) return;
            //  согласные буквы
            string consonants = "бвгджзклмнпрстфхцчшщbcdfghklmnpqrstvwxz";
            //  гласные буквы
            string vowel = "аеёийоуыэюяaeijouy";
            //  если удаляем гласные, присваиваем в переменную с  гласными в перменную с согласными
            if (charType== CharType.Vowel) 
                consonants = vowel;
            //  пребираем список
            for (int i = 0; i < letters.Count; i++) {                
                if (consonants.Contains(letters[i].Letter.ToLower()[0])) {
                    //  если нашли ,удаляем из списка
                    // т.к. мы удаляем элемент списка то надо уменьшить индекс чтобы не пропуститьследующий элемент
                    letters.RemoveAt(i--);
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
        private static void PrintStatistic(IEnumerable<LetterStats> letters) {
            if (!letters.Any()) return;
            //  сортируем
            LetterStats[] sls = letters.OrderBy(a => a.Letter).ToArray();
            //  общее кол-во найденных букв/ пар
            int sum = 0;
            foreach (var ls in sls) {
                Console.WriteLine($"{ls.Letter} {ls.Count}");
                sum += ls.Count;
            }
            Console.WriteLine($"общее кол-во букв/пар { sum}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(ref LetterStats letterStats) {
            letterStats.Count++;
        }


    }
}
