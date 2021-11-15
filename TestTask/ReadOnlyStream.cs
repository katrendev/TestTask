using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private readonly Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            try
            {
                _localStream = new FileStream(fileFullPath, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось открыть файл! Error: {e}");
            }
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof { get; private set; }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public string ReadNextChar(int position)
        {
            if (position > _localStream.Length)
            {
                ResetPositionToStart();
                IsEof = true;
                return null;
            }

            _localStream.Seek(position, SeekOrigin.Begin);
            var output = new byte[1];
            _localStream.Read(output, 0, 1);
            var result = Encoding.Default.GetString(output);

            return char.IsLetter(result[0]) ? result : null;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;

                return;
            }

            _localStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Закрытие файла
        /// </summary>
        public void CloseToFile()
        {
            _localStream.Close();
        }
    }
}