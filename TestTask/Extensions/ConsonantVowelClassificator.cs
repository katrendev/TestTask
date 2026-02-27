namespace TestTask.Extensions;
/// <summary>
/// Классификатор гласных и согласных. Определяет русские и английские буквы. Откидывает мягкий и твердый знаки.
/// </summary>
public static class ConsonantVowelClassificator
{
    private static readonly HashSet<char> Vowels = new HashSet<char>("аеёиоуыэюяАЕЁИОУЫЭЮЯaeiouAEIOU");
    private static readonly HashSet<char> UncategorizedChars = new HashSet<char>("ьъЬЪ");

    /// <summary>
    /// Является ли гласной.
    /// </summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns></returns>
    public static bool IsVowel(char c)
    {
        if (!char.IsLetter(c))
            return false;
        if (UncategorizedChars.Contains(c))
            return false;
        return Vowels.Contains(c);
    }
    /// <summary>
    /// Является ли согласной.
    /// </summary>
    /// <param name="c">Проверяемый символ</param>
    /// <returns></returns>
    public static bool IsConsonant(char c)
    {
        if (!char.IsLetter(c))
            return false;
        if (UncategorizedChars.Contains(c))
            return false;
        return !Vowels.Contains(c);
    }
}