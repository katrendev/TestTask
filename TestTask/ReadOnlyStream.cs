using System;
using System.IO;

namespace TestTask
{
	public class ReadOnlyStream : IReadOnlyStream
	{
		private StreamReader _localStream;

		/// <summary>
		/// Конструктор класса. 
		/// Т.к. происходит прямая работа с файлом, необходимо 
		/// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		public ReadOnlyStream(string fileFullPath)
		{
			// Заменить на создание реального стрима для чтения файла!
			_localStream = new StreamReader(fileFullPath);
			
			//IsEof = true;
		}

		/// <summary>
		/// Флаг окончания файла.
		/// </summary>
		// Заполнять данный флаг при достижении конца файла/стрима при чтении
		public bool IsEof => _localStream.EndOfStream;

		/// <summary>
		/// Ф-ция чтения следующего символа из потока.
		/// Если произведена попытка прочитать символ после достижения конца файла, метод 
		/// должен бросать соответствующее исключение
		/// </summary>
		/// <returns>Считанный символ.</returns>
		public char ReadNextChar()
		{
			// Необходимо считать очередной символ из _localStream
			if (_localStream.EndOfStream)
				throw new Exception("Не удалось прочиать следующий символ, т.к. достигнут конец потока.");
			return (char) _localStream.Read();
		}

		/// <summary>
		/// Сбрасывает текущую позицию потока на начало.
		/// </summary>
		public void ResetPositionToStart()
		{
			//if (_localStream == null)
			//{
			//	IsEof = true;
			//	return;
			//}

			_localStream.BaseStream.Position = 0;
		}

		public void Dispose()
		{
			try { _localStream.Close(); }
			finally { _localStream.Dispose(); }
		}
	}
}
