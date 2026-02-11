using System;

namespace TestTask.Helpers
{
    public static class HelperMain
    {
        private const string __help = "Пожалуйста введите 2 аргумента командной строки! Пути до файлов для обработки.";
        private const string __pressAnyKey = "Для закрытия окна, нажмите любую клавишу...";

        /// <summary>
        /// Метод проверяет подхолит ли массив аргументов.
        /// </summary>
        /// <param name="args">Входные аргументы.</param>
        /// <returns>Является ли аргументы не подходящими.</returns>
        public static bool IsBadArgs(string[] args) =>
            args == null || args.Length < 2 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]);

        /// <summary>
        /// Метод печатает в консоли подсказку по использованию программы.
        /// </summary>
        public static void PrintHelp() => PrintToConsole(__help);

        /// <summary>
        /// Метод печатает в консоли сообщение о нажатии клавиши и ждет от пользователя нажатия клавишы.
        /// </summary>
        public static void WaitPressKey()
        {
            PrintToConsole(string.Empty);
            PrintToConsole(__pressAnyKey);
            Console.ReadKey();
        }

        /// <summary>
        /// Метод печатает в консоль переданную строку.
        /// </summary>
        /// <param name="report">Строка, которую нужно напечатать.</param>
        public static void PrintToConsole(string report) =>
            Console.WriteLine(report);
    }
}
