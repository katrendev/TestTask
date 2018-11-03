namespace TestTask.CharacterMachines
{
    /// <summary>
    /// Интерфейс для автомата, работающего с символами. Автомату последовательно подаются символы методом
    /// CheckChar, если после подачи данного символа метод возвращает true, то заносим в статистику строку
    /// GetCurrentString.
    /// </summary>
    internal interface ICharacterMachine
   {
       string GetCurrentString();

       bool CheckChar(char c);

       void Reset();
   }
}
