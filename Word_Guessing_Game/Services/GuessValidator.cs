
using Word_Guessing_Game.Exceptions;


namespace Word_Guessing_Game.Services
{
    public class GuessValidator
    {
      
      public void ValidateGuess(string guess)
        {

            //input validations
            if(guess.Length == 0)
            {
                throw new InvalidCastException("Guess cannot be empty.");
            }
            if(guess.Length < 5)
            {
                throw new InvalidGuessException("Guess must be 5 letters long.");
            }
            if(guess.Length > 5)
            {
                throw new InvalidGuessException("Guess must be 5 letters long.");
            }

            foreach(char c in guess)
            {
                if (char.IsDigit(c))
                {
                    throw new InvalidGuessException("Guess cannot contain numbers.");
                }
                if (!char.IsLetter(c))
                {
                    throw new InvalidGuessException("Guess cannot contain special characters.");
                }
            }
        }
    }
}