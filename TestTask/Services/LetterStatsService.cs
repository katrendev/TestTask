using TestTask.Extensions;
using TestTask.Interfaces;
using TestTask.Models;

namespace TestTask.Services;

/// <summary>
/// Сервис для сбора статистики букв из потока.
/// </summary>
public static class LetterStatsService
{
    /// <summary>
    /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
    /// Статистика РЕГИСТРОЗАВИСИМАЯ!
    /// </summary>
    /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
    /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
    public static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
    {
        stream.ResetPositionToStart();
        var stats = new Dictionary<char, int>();

        while (!stream.IsEof)
        {
            char c = stream.GetNextLetter();
            if (!char.IsLetter(c)) break;

            stats[c] = stats.GetValueOrDefault(c) + 1;
        }

        return stats
            .Select(s => new LetterStats(s.Key.ToString(), s.Value))
            .ToList();
    }

    /// <summary>
    /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
    /// В статистику должны попадать только пары из одинаковых букв, например АА, сС, УУ, ee и т.д.
    /// Статистика - НЕ регистрозависимая!
    /// </summary>
    /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
    /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
    public static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
    {
        stream.ResetPositionToStart();
        var stats = new Dictionary<string, int>();

        if (stream.IsEof)
            return new List<LetterStats>();

        char prevChar = stream.GetNextLetter();
        
        if (!char.IsLetter(prevChar))
            return new List<LetterStats>();
        
        while (!stream.IsEof )
        {
            char c = stream.ReadNextChar();

            if (!char.IsLetter(c))
            {
                prevChar = '\0';
                continue;
            }

            if (!prevChar.EqualsWithCaseOption(c, true))
            {
                prevChar = c;
                continue;
            }

            var combination = new string(new[] { prevChar.ToLowerInvariant(), c.ToLowerInvariant() });
            
            stats[combination] = stats.GetValueOrDefault(combination) + 1;
            prevChar = c;
        }

        return stats
            .Select(s => new LetterStats(s.Key, s.Value))
            .ToList();
    }
}