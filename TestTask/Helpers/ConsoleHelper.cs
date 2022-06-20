using System;

namespace TestTask.Helpers
{
    /// <summary>
    /// Помощник в работе с консолью.
    /// Необходим для контроля работы с консолью.
    /// </summary>
    internal sealed class ConsoleHelper
    {
        #region Public Methods

        /// <summary>
        /// Выводит в консоль строку.
        /// </summary>
        /// <param name="text">Текст, который необходимо вывести.</param>
        public static void Write(string text)
        {
            Console.WriteLine(text);
        }

        /// <summary>
        /// Считывает нажатую клавишу.
        /// </summary>
        /// <returns>Возвращает информацию о нажатой клавише.</returns>
        public static ConsoleKeyInfo ReadKey() => Console.ReadKey();

        #endregion Public Methods
    }
}