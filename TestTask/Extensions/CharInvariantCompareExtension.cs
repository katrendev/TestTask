namespace TestTask.Extensions;

public static class CharInvariantCompareExtension
{ 
    /// <summary>
    /// Проверка символов без учета регистра.
    /// </summary>
    /// <param name="c1">Символ, относительно которого сравниваем.</param>
    /// <param name="c2">Символ сравнения.</param>
    /// <param name="ignoreCase">Учитывать ли регистр.</param>
    public static bool EqualsWithCaseOption(this char c1, char c2, bool ignoreCase = false)
    {
        if (ignoreCase)
            return c1.ToLowerInvariant().Equals(c2.ToLowerInvariant());
                
        return c1.Equals(c2);
    }

    /// <summary>
    /// Приведение к нижнему регистру.
    /// </summary>
    /// <param name="c">Символ приведения.</param>
    /// <returns></returns>
    public static char ToLowerInvariant(this char c)
    {
        return char.ToLowerInvariant(c);
    }
}