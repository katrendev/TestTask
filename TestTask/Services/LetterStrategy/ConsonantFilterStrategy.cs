using TestTask.Extensions;

namespace TestTask.Services.LetterStrategy;

/// <summary>
/// Стратегия удаления согласных букв из статистики.
/// </summary>
public class ConsonantFilterStrategy : ICharFilterStrategy
{
    public bool ShouldRemove(string letter)
    {
        if (string.IsNullOrEmpty(letter)) return false;
        return ConsonantVowelClassificator.IsConsonant(letter[0]);
    }
}