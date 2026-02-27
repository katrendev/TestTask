namespace TestTask.Streams.Interfaces
{
	using System;

	/// <summary>
	/// Интерфейс для работы с файлом в сильно урезаном виде.
	/// </summary>
	internal interface IReadOnlyStream : IDisposable
	{
		/// <summary>
		/// Читает следующий символ из потока.
		/// </summary>
		/// <returns>Считанный символ.</returns>
		char ReadNextChar();

		/// <summary>
		/// Сбрасывает текущую позицию потока на начало.
		/// </summary>
		void ResetPositionToStart();

		/// <summary>
		/// Флаг окончания файла.
		/// </summary>
		bool IsEof { get; }
	}
}