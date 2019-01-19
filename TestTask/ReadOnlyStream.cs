using System;
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
            try
            {
                _localStream = new FileInfo(fileFullPath).OpenRead();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Путь к файлу не найден! Текст ошибки: {e.Message}");
                Console.WriteLine("Нажмите любую клавишу для завершения!");
                Console.ReadKey();

                Environment.Exit(0);
            }
            // TODO : Заменить на создание реального стрима для чтения файла!
        }

        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get
            {
                if (_localStream != null)
                {
                    return _localStream.Position == _localStream.Length;
                }
                return true;
                // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            }
        }


        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            //Если конец файла, то сгенерировать специальное исключение
            try
            {
                if (IsEof)
                    throw new StreamIsDeadException("Попытка прочитать символ после достижения конца файла!!!");
                int b = _localStream.ReadByte();
                return b == -1 ?
                    throw new StreamIsDeadException("Попытка прочитать символ после достижения конца файла!!!")
                    : Encoding.Default.GetChars(new byte[] { (byte)b })[0];
            }
            catch (StreamIsDeadException e)
            {
                Console.WriteLine(e.Message);
                return default;
            }

            // TODO : Необходимо считать очередной символ из _localStream
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                return;
            }

            _localStream.Position = 0;
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            _localStream.Dispose();
        }
    }
}
