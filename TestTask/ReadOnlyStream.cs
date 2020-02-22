using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;

            // Заменить на создание реального стрима для чтения файла!
            _localStream = File.OpenRead(fileFullPath);
        }
        /// <summary>
        /// Деструктор, закрывает файл
        /// </summary>
        ~ReadOnlyStream()
        {
            Close();
        }
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; //Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        }
        /// <summary>
        /// Закрыть файл
        /// </summary>
        public void Close()
        {
            IsEof = true;
            _localStream.Close();
            _localStream = null;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            if (IsEof) new FileLoadException();
            if (_localStream == null) new FileNotFoundException();
            
            var result = new byte[2];

            IsEof = _localStream.Read(result, 0, 2) <= 0;
            
            return Encoding.UTF8.GetChars(result)[0];
            
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
    }
}
