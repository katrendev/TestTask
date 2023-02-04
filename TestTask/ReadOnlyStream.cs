using System;
using System.IO;

namespace TestTask
{
	public class ReadOnlyStream : IReadOnlyStream
	{
		#region Private Fields

		/// <summary>
		/// Было ли освобождение ресурсов.
		/// </summary>
		private bool _isDisposed = false;

		private Stream _localStream;

		private StreamReader _localStreamReader;

		#endregion Private Fields

		#region Public Properties

		/// <summary>
		/// Флаг окончания файла.
		/// </summary>
		public bool IsEof => _localStreamReader.EndOfStream;

		#endregion Public Properties

		#region Public Constructors

		/// <summary>
		/// Конструктор класса.
		/// Т.к. происходит прямая работа с файлом, необходимо
		/// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		public ReadOnlyStream(string fileFullPath)
		{
			InitializeComponents(fileFullPath);
		}

		#endregion Public Constructors

		#region Private Destructors

		/// <summary>
		/// Деструктор.
		/// </summary>
		~ReadOnlyStream() => Dispose(false);

		#endregion Private Destructors

		#region Public Methods

		/// <summary>
		/// Реализация интерфейса IDisposable.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Ф-ция чтения следующего символа из потока.
		/// Если произведена попытка прочитать символ после достижения конца файла, метод
		/// должен бросать соответствующее исключение
		/// </summary>
		/// <returns>Считанный символ.</returns>
		public string ReadNextChar()
		{
			if (IsEof)
			{
				throw new EndOfStreamException();
			}

			char currentLetter = (char)_localStreamReader.Read();

			return currentLetter.ToString();
		}

		/// <summary>
		/// Сбрасывает текущую позицию потока на начало.
		/// </summary>
		public void ResetPositionToStart()
		{
			_localStream.Position = 0;
		}

		#endregion Public Methods

		#region Protected Methods

		/// <summary>
		/// Освобождает ресурсы.
		/// </summary>
		protected virtual void Dispose(bool isManualDisposal)
		{
			if (!_isDisposed)
			{
				if (isManualDisposal)
				{
					_localStreamReader.Dispose();
					_localStream.Dispose();
				}

				_isDisposed = true;
			}
		}

		#endregion Protected Methods

		#region Private Methods

		/// <summary>
		/// Инициализирует компоненты объекта класса <see cref="ReadOnlyStream"/>.
		/// </summary>
		private void InitializeComponents(string fileFullPath)
		{
			_localStream = File.Open(fileFullPath, FileMode.Open, FileAccess.Read);
			_localStreamReader = new StreamReader(_localStream);
		}

		#endregion Private Methods
	}
}