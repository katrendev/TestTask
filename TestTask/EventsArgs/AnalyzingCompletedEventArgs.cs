﻿using System;
using System.Collections.Generic;
using TestTask.Models;

namespace TestTask.EventsArgs
{
    /// <summary>
    /// Параметры события завершения сканирования файла.
    /// </summary>
    internal class AnalyzingCompletedEventArgs : EventArgs
    {
        #region Public Constructors

        /// <summary>
        /// Инициализирует экземпляр <see cref="AnalyzingCompletedEventArgs"/>.
        /// </summary>
        /// <param name="result">Результаты анализа.</param>
        public AnalyzingCompletedEventArgs(IEnumerable<EntryStats> result)
        {
            Result = result;
        }

        #endregion Public Constructors

        #region Private Properties

        /// <summary>
        /// Результаты анализа.
        /// </summary>
        public IEnumerable<EntryStats> Result { get; }

        #endregion Private Properties
    }
}