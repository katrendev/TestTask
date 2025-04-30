using System;
using System.IO;

namespace TestTask
{
    internal class FileReader : IReadOnlyStream
    {
        private StreamReader reader;
        private string filePath;

        public FileReader(string filePath)
        {
            if (File.Exists(filePath))
            {
                this.filePath = filePath;
                reader = new StreamReader(filePath);
            }
            else
            {
                throw new Exception("Файл не найден: " + filePath);
            }
        }

        public char ReadNextChar()
        {
            if (reader == null)
                throw new Exception("Reader не инициализирован");

            int symbol = reader.Read();

            if (symbol == -1)
            {
                throw new Exception("Достигнут конец файла");
            }

            return Convert.ToChar(symbol);
        }

        public void ResetPositionToStart()
        {
            if (reader != null)
            {
                reader.Close(); // на всякий случай
                reader = new StreamReader(filePath);
            }
        }

        public bool IsEof
        {
            get
            {
                if (reader != null)
                    return reader.EndOfStream;
                return true;
            }
        }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
        }
    }
}
