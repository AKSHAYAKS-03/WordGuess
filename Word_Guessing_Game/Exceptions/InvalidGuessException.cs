namespace Word_Guessing_Game.Exceptions
{
    public class InvalidGuessException : Exception
    {
        public InvalidGuessException(string message): base(message)
        {
        }
        public InvalidGuessException(string message, Exception innerException): base(message, innerException)
        {
        }

    }
}