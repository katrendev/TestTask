using TestTask.Extensions;

namespace TestTask.Services.LetterStrategy;

/// <summary>
/// Стратегия удаления гласных букв из статистики.
/// </summary>
public class VowelFilterStrategy : ICharFilterStrategy
{
    public bool ShouldRemove(string letter)
    {
        if (string.IsNullOrEmpty(letter)) return false;
        return ConsonantVowelClassificator.IsVowel(letter[0]);
    }
}