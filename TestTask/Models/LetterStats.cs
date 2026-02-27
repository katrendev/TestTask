namespace TestTask.Models
{
    /// <summary>
    /// Статистика вхождения буквы/пары букв
    /// </summary>
    public record struct LetterStats (string Letter, int Count = 0);
}
