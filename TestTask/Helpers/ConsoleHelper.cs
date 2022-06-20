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
        /// Считывает нажатую клавишу.
        /// </summary>
        /// <returns>Возвращает информацию о нажатой клавише.</returns>
        public static ConsoleKeyInfo ReadKey() => Console.ReadKey();

        /// <summary>
        /// Выводит в консоль строку.
        /// </summary>
        /// <param name="text">Текст, который необходимо вывести.</param>
        public static void WriteLine(string text, params object[] parameters)
        {
            Console.WriteLine(text, parameters);
        }

        #endregion Public Methods
    }
}