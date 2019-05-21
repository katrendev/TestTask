using System.Collections.Generic;

namespace TestTask
{
    public interface IAnalizer
    {      
       List<AnalizeDataModel> GetTextAnalize(string analizedText, List<string> valueForSearchList);
    }
}
