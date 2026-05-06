namespace Word_Guesing_Game.Exceptions
{
    public class InvalidGuessException : Exception
    {
        public InvalidGuessException(string message) : base(message) { }
    }
}