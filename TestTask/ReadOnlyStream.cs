using System;
using System.IO;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private Stream _localStream;

        private StreamReader _localStreamReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            // TODO : Заменить на создание реального стрима для чтения файла!
            _localStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            _localStreamReader = new StreamReader(_localStream);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get; // TODO : Заполнять данный флаг при достижении конца файла/стрима при чтении
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {
            // TODO : Необходимо считать очередной символ из _localStream

            var nextValue = _localStreamReader.Read(); 

            char nextChar = new char();

            if (nextValue != -1 || !IsEof)
            {
                /*
                  *  Не совсем понял как вы хотите чтобы обрабатывалось исключение при попытке 
                  *  прочитать следующий символ после окончания файла, поэтому попытался сделать дословно.
                  *  
                  *  Кроме того я не был уверен в том через что вы хотите, чтобы я сделал получение очередного
                  *  символа, через FileStream или через StreamReader. Я видел в описании примеры на кириллице,
                  *  поэтому сделал через FileStream, потому что через StreamReader с ней были бы проблемы.
                  */

                try
                {
                    nextChar = (char)nextValue;
                }
                catch
                {
                    throw new IndexOutOfRangeException("File is complete !");
                }
            }
            else
            {
                IsEof = true;
            }    

            return nextChar;
        }

        /// <summary>
        /// Сбрасывает флаг окончания чтения.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localStream == null)
            {
                IsEof = true;
                return;
            }

            IsEof = false;
        }
    }
}
