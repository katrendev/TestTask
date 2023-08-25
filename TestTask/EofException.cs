namespace TestTask
{
    public class EofException : Exception
    {
        public EofException() : base("Can't read from stream: there is EOF.") {}
    }
}