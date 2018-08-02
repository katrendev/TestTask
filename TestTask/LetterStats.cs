namespace TestTask
{
    public class LetterStats
    {
        public LetterStats(char letter)
        {
            Letter = letter;
            Count = 0;
        }
        
        public void IncCount()
        {
            Count++;
        }

        public char Letter { get; }

        public int Count { get; private set; }
    }
}