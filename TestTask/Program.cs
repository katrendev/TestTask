using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TestTask
{
	public class Program
	{

        /// <summary>
        /// Программа принимает на входе 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
		{
			if(args.Length != 2) {
				Console.WriteLine("Неправильно указаны пути файлов. Необходимо указать в качестве аргументов запуска два пути до конечных файлов." + "\n" 
					+ "Нажмите любую клавишу для выхода из программы...");
				Console.ReadKey();
				return;
			}

			IList<LetterStats> singleLetterStats;
			IList<LetterStats> doubleLetterStats;

			try {
				using (IReadOnlyStream firstInputStream = GetInputStream(args[0]))
					singleLetterStats = FillSingleLetterStats(firstInputStream);
				using (IReadOnlyStream secondInputStream = GetInputStream(args[1]))
					doubleLetterStats = FillDoubleLetterStats(secondInputStream);
			}
			catch (Exception e) {
				Console.WriteLine("В процессе чтения из файла произошла ошибка: " + e.Message + "\n" 
					+ "Нажмите любую клавишу для выхода из программы...");
				Console.ReadKey();
				return;
			}

			RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
			RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

			PrintStatistic(singleLetterStats);
			PrintStatistic(doubleLetterStats);

			// Необжодимо дождаться нажатия клавиши, прежде чем завершать выполнение программы.
			Console.WriteLine("\nРабота программы завершена, нажмите любую клавишу для выхода из программы...");
			Console.ReadKey();
		}

		/// <summary>
		/// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
		/// </summary>
		/// <param name="fileFullPath">Полный путь до файла для чтения</param>
		/// <returns>Поток для последующего чтения.</returns>
		private static IReadOnlyStream GetInputStream(string fileFullPath)
		{
			return new ReadOnlyStream(fileFullPath);
		}

		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
		/// Статистика РЕГИСТРОЗАВИСИМАЯ!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();

			var letterStatsList = new List<LetterStats>();

			while (!stream.IsEof)
			{
				// заполнять статистику с использованием метода IncStatistic. Учёт букв - регистрозависимый.
				char c = stream.ReadNextChar();
				
				if (char.IsLetter(c))
					if (!letterStatsList.Exists(lS => lS.Letter == c.ToString()))
						letterStatsList.Add(new LetterStats {Letter = c.ToString(), Count = 1});
					else {
						int index = letterStatsList.FindIndex(lS => lS.Letter == c.ToString());
						LetterStats letterStats = letterStatsList[index];
						IncStatistic(ref letterStats);
						letterStatsList[index] = letterStats;
					}
			}

			return letterStatsList;
		}

		/// <summary>
		/// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
		/// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
		/// Статистика - НЕ регистрозависимая!
		/// </summary>
		/// <param name="stream">Стрим для считывания символов для последующего анализа</param>
		/// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
		private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
		{
			stream.ResetPositionToStart();

			var letterStatsList = new List<LetterStats>();

			char letter = stream.ReadNextChar();
			
			while (!stream.IsEof)
			{
				// заполнять статистику с использованием метода IncStatistic. Учёт букв - НЕ регистрозависимый.
				char c = stream.ReadNextChar();
				
				if(char.IsLetter(c) && char.ToUpper(c) == char.ToUpper(letter))
					if(!letterStatsList.Exists(lS => lS.Letter[0] == char.ToUpper(c)))
						letterStatsList.Add(new LetterStats {Letter = new string(char.ToUpper(c), 2), Count = 1});
					else {
						int index = letterStatsList.FindIndex(lS => lS.Letter == new string(char.ToUpper(c), 2));
						LetterStats letterStats = letterStatsList[index];
						IncStatistic(ref letterStats);
						letterStatsList[index] = letterStats;
					}

				letter = c;
			}

			return letterStatsList;
		}

		/// <summary>
		/// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
		/// (Тип букв для перебора определяется параметром charType)
		/// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
		/// </summary>
		/// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
		/// <param name="charType">Тип букв для анализа</param>
		private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
		{
			// Удалить статистику по запрошенному типу букв.
			LetterStats[] letterStatses;
			switch(charType)
			{
				case CharType.Vowel:
					char[] vowels = { 'А', 'а', 'У', 'у', 'О', 'о', 'Ы', 'ы', 'И', 'и', 'Э', 'э', 'Я', 'я', 'Ю', 'ю', 'Ё', 'ё', 'Е', 'е' };
					letterStatses = (from letterStats in letters where vowels.Contains(letterStats.Letter[0]) select letterStats).ToArray();
					for (int i = 0; i < letterStatses.Length; i++)
						letters.Remove(letterStatses[i]);
					break;
				case CharType.Consonants:
					char[] consonants = {'Б', 'б', 'В', 'в', 'Г', 'г', 'Д', 'д', 'Ж', 'ж', 'З', 'з', 'Й', 'й', 'К', 'к', 'Л', 'л', 'М',
						'м', 'Н', 'н', 'П', 'п', 'Р', 'р', 'С', 'с', 'Т', 'т', 'Ф', 'ф', 'Х', 'х', 'Ц', 'ц', 'Ч', 'Ш', 'ш', 'Щ', 'щ'};
					letterStatses = (from letterStats in letters where consonants.Contains(letterStats.Letter[0]) select letterStats).ToArray();
					for (int i = 0; i < letterStatses.Length; i++)
						letters.Remove(letterStatses[i]);
					break;
			}
		}

		/// <summary>
		/// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
		/// Каждая буква - с новой строки.
		/// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
		/// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
		/// </summary>
		/// <param name="letters">Коллекция со статистикой</param>
		private static void PrintStatistic(IEnumerable<LetterStats> letters)
		{
			// Выводить на экран статистику. Выводить предварительно отсортировав по алфавиту!
			Console.WriteLine("");
			letters = letters.OrderBy(l => l.Letter);
			foreach(LetterStats letter in letters)
				Console.WriteLine(letter.Letter + " : " + letter.Count);
			Console.WriteLine("ИТОГО : " + letters.Sum(l => l.Count));
			
		}

		/// <summary>
		/// Метод увеличивает счётчик вхождений по переданной структуре.
		/// </summary>
		/// <param name="letterStats"></param>
		private static void IncStatistic(ref LetterStats letterStats)
		{
			letterStats.Count++;
		}

	}
}
