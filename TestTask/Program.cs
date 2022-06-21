using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTask.Data.English;
using TestTask.Enums;
using TestTask.Helpers;
using TestTask.Models;
using TestTask.Services;
using TestTask.Streams;
using TestTask.Streams.Interfaces;

namespace TestTask
{
    public class Program
    {
        #region Private Methods







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
            //TODO раскомментировать.
            //if (args.Length == 0)
            //{
            //    ConsoleHelper.Write("Args was empty");
            //    return;
            //}

            var a = new StatisticService();
            a.SetFilePath(@"C:\Users\mikhi\Desktop\dev\1.txt");
            a.StartAnalyzing();
            

            ConsoleHelper.ReadKey();
        }



        #endregion Private Methods
    }
}