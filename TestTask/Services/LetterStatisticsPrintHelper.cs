using TestTask.Models;

namespace TestTask.Services;

public static class LetterStatisticsPrintHelper
{
    /// <summary>
    /// Ф-ция выводит на экран полученную статистику в формате "{Буква/пара} : {Кол-во}"
    /// Каждая буква/пара - с новой строки.
    /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
    /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
    /// </summary>
    /// <param name="letters">Коллекция со статистикой</param>
    public static void PrintStatisticSorted(IList<LetterStats> letters)
    {
        var sorted = letters.OrderBy(x => x.Letter.ToLowerInvariant());
        int total = 0;
        foreach (var letter in sorted)
        {
            Console.WriteLine($"{letter.Letter} : {letter.Count}");
            total += letter.Count;
        }
        
        Console.WriteLine($"ИТОГО : {total}");
    }
}