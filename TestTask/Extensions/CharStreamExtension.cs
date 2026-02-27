using TestTask.Interfaces;

namespace TestTask.Extensions;

internal static class CharStreamExtension
{
    /// <summary>
    /// Проходит по потоку и находит первый символ являющийся буквой по Unicode.
    /// </summary>
    /// <param name="stream">Поток текста.</param>
    /// <returns>Буква или символ новой строки(конец потока).</returns>
    public static char GetNextLetter(this IReadOnlyStream stream)
    {
        char c = '\0';

        while (!stream.IsEof && !char.IsLetter(c))
            c = stream.ReadNextChar();
            
        return c;
    }
}