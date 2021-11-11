﻿using System;
using System.Collections.Generic;
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

            //RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            //RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            // TODO : Необходимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
            //
            Console.ReadKey();
            //
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
            List<LetterStats> singleLetterStats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
                //
                string symbol = c.ToString();
                Regex alphabet = new Regex(@"[а-яА-Я]");
             
                if (alphabet.IsMatch(symbol))
                {
                    int index = singleLetterStats.FindIndex((item) => item.Letter == symbol);
                    if (index != -1)
                    {
                        IncStatistic(singleLetterStats, index);
                        Console.WriteLine(singleLetterStats[index].Letter + singleLetterStats[index].Count);
                    } 
                    else
                    {
                        singleLetterStats.Add(new LetterStats { Letter = symbol, Count = 1 });
                    }
                }
            //
            }
            //return ???;

            return singleLetterStats;
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
            List<LetterStats> doubleLetterStats = new List<LetterStats>();
            while (!stream.IsEof)
            {
                char c = stream.ReadNextChar();
                // TODO : заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
                //
                string symbol = c.ToString().ToUpper();
                Regex alphabet = new Regex(@"[А-Я]");
                if (alphabet.IsMatch(symbol))
                {
                    string pair = symbol.Insert(1, symbol.ToLower());
                    int index = doubleLetterStats.FindIndex((item) => item.Letter == pair);
                    if (index != -1)
                    {
                        IncStatistic(doubleLetterStats, index);
                    }
                    else
                    {
                        doubleLetterStats.Add(new LetterStats { Letter = pair, Count = 1 });
                    }
                }
                //
            }
            //return ???;
            return doubleLetterStats;
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
            // TODO : Удалить статистику по запрошенному типу букв.
            switch (charType)
            {
                case CharType.Consonants:
                    break;
                case CharType.Vowel:
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
            // TODO : Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
            //
            int totalSum = 0;
            foreach (LetterStats ls in letters)
            {
                totalSum += ls.Count;
                Console.WriteLine(ls.Letter + " : " + ls.Count);
            }

            //
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(List<LetterStats> letterStats, int index)
        {
            int tempCount = letterStats[index].Count;
            string tempLetter = letterStats[index].Letter;
            letterStats.RemoveAt(index);
            letterStats.Add(new LetterStats { Letter = tempLetter, Count = ++tempCount }); 
        }


    }
}
