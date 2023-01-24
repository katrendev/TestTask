using System.Text;
public class PairFounder
{
    private string prevChar = null;
    private int count = 0;
    public string resultPair;
    /// <summary>
    /// Ф-ция поверяющая по входящей букве была ли последовательность перед ней из одинаковых букв 
    /// ровно из двух элементов
    /// </summary>
    /// <param name="item">Новый считанный из стрима символ</param>
    /// <returns>Пара одинаковых букв приведенная в верхней регистр</returns>
    public string CheckLetter(string item)
    {
        item = item.ToUpper();
        resultPair = null;
        if (prevChar == item)
        {
            count++;
        }
        else
        {
            if (count == 2)
            {
                resultPair = new StringBuilder(prevChar + prevChar).ToString();
            }
            count = 1;
            prevChar = item;
        }
        return resultPair;
    }
    /// <summary>
    /// Ф-ция поверяющая по входящей букве была ли последовательность перед ней из одинаковых букв 
    /// ровно из двух элементов но в отличие от CheckLetter не дожидается считывания следующего символа 
    /// чтоб определить пару букв
    /// </summary>
    /// <returns>Пара одинаковых букв приведенная в верхней регистр</returns>
    public string CheckLetterLast(string item)
    {
        item = item.ToUpper();
        resultPair = null;
        if (prevChar == item)
        {
            count++;
            if (count == 2)
            {
                resultPair = new StringBuilder(prevChar + prevChar).ToString();
            }
        }
        return resultPair;
    }
}