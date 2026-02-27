using TestTask.Models;

namespace TestTask.Services.LetterStrategy;

public static class CharFilterStrategyFactory
{
    private static readonly Dictionary<CharType, ICharFilterStrategy> Strategies = new()
    {
        { CharType.Vowel,      new VowelFilterStrategy() },
        { CharType.Consonants, new ConsonantFilterStrategy() },
    };
    
    /// <summary>
    /// Возвращает стратегию фильтрации для указанного типа символов.
    /// </summary>
    /// <param name="charType">Тип символов.</param>
    /// <returns>Соответствующая стратегия.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Если тип не поддерживается.</exception>
    public static ICharFilterStrategy GetStrategy(CharType charType)
    {
        if (!Strategies.TryGetValue(charType, out var strategy))
            throw new ArgumentOutOfRangeException(nameof(charType), $"Неизвестный тип: {charType}");

        return strategy;
    }
}