namespace TestTask.Services.LetterStrategy;

/// <summary>
///  Стратегия выбора фильтров букв из статистики
/// </summary>
public interface ICharFilterStrategy
{
    /// <summary>
    /// Определяет, надо ли убрать букву/пару из статистики
    /// </summary>
    /// <param name="letter">Буква или пара букв для проверки</param>
    /// <returns>true - если надо удалить</returns>
    bool ShouldRemove(string letter);
}