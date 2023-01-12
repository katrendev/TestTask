using System;
using System.IO;
using System.Text;
using TestTask.Services.Interfaces;

namespace TestTask.Services
{
    /// <summary>
    /// Работа с файлами реализована через FileStream
    /// </summary>
    public class ReadOnlyStreamReader : IReadOnlyStream
    {
        /// <summary>
        /// Поток для чтения
        /// </summary>
        private StreamReader _localStream;

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get => _localStream?.EndOfStream ?? true;
        }
        /// <summary>
        /// Конструктор класса. 
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStreamReader(string fileFullPath)
        {
            if(File.Exists(fileFullPath)) _localStream = new StreamReader(fileFullPath);           
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof) throw new EndOfStreamException();

            return (char)_localStream.Read();
        }   

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null) return;

            _localStream.BaseStream.Position = 0;
        }

        /// <summary>
        /// Освобождение ресурсов (закрываем поток)
        /// </summary>
        public void Dispose()=> _localStream?.Dispose();
    }
}
