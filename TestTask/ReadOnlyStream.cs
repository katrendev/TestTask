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
			//IsEof = true;

			// TODO : Заменить на создание реального стрима для чтения файла!
			try
			{
				_localStreamReader = new StreamReader(fileFullPath, System.Text.Encoding.Default);

				IsEof = _localStreamReader.EndOfStream;

				if (IsEof)
					_localStreamReader.Close();

			}
			catch (Exception e)
			{
				Console.WriteLine("The process failed: {0}", e.ToString());
			}

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

			if (IsEof)
				throw new EndOfStreamException();

			var c = (char) _localStreamReader.Read();

			if (_localStreamReader.Peek() < 0)
			{
				IsEof = true;
				_localStreamReader.Close();
			}

			return c;
		}

		/// <summary>
		/// Сбрасывает текущую позицию потока на начало.
		/// </summary>
		public void ResetPositionToStart()
		{
			if (_localStreamReader == null)
			{
				IsEof = true;
				return;
			}

			_localStreamReader.BaseStream.Position = 0;
			IsEof = false;
		}
	}

	class EndOfFileException : ArgumentException
	{
		public EndOfFileException(string message) : base(message)
		{

		}
	}
}
