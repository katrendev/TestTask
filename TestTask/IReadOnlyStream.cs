﻿using System;

namespace TestTask
{
    /// <summary>
    /// Интерфейс для работы с файлом в сильно урезаном виде.
    /// Умеет всего 2 вещи: прочитать символ, и перемотать стрим на начало.
    /// </summary>
    internal interface IReadOnlyStream : IDisposable
    {
        // TODO : Необходимо доработать данный интерфейс для обеспечения гарантированного закрытия файла, по окончанию работы с таковым!
        char ReadNextChar();

        void ResetPositionToStart();

        bool IsEof { get; }


    }
}
