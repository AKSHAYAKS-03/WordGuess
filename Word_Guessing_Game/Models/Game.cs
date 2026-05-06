using System;
using System.Collections.Generic;
using Word_Guessing_Game.Services;
using Word_Guessing_Game.Exceptions;

namespace Word_Guessing_Game.Models
{
    public class Game
    {
        private readonly WordProvider _wordProvider;
        private readonly GuessValidator _guessValidator;
        private readonly FeedbackGenerator _feedbackGenerator;


        private List<string> _guesses;
        private string _hiddenword;
        private int _score;

        public Game()
        {
            _wordProvider = new WordProvider();
            _guessValidator = new GuessValidator();
            _feedbackGenerator = new FeedbackGenerator();

            _guesses = new List<string>();
            _hiddenword = _wordProvider.GetRandomWord();
            _score = 0;
        }


        public void StartGame()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==================== WORD GUESSS GAME ====================");
            Console.WriteLine();
            Console.WriteLine("Welcome to the Word Guessing Game!");
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Choose Difficulty");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");

            Console.WriteLine("Enter choice: ");

            int difficulty = Convert.ToInt32(Console.ReadLine() ?? "1");
            int maxAttempts = getMaxAttempts(difficulty);

            Console.WriteLine($"\nYou have {maxAttempts} attempts to guess the word.\n");

            bool correctguess = false;

            int attempts;
            for(attempts = 1; attempts <= maxAttempts; attempts++)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Attempt {attempts}: Enter your guess: ");
                    string guess = (Console.ReadLine() ?? "").ToUpper();

                    _guessValidator.ValidateGuess(guess);

                    if (_guesses.Contains(guess))
                    {
                        throw new InvalidGuessException("You have already guessed that word.");
                    }
                    _guesses.Add(guess);
                    string feedback = _feedbackGenerator.GenerateFeedback(_hiddenword, guess);

                    DisplayFeedback(guess, feedback);

                    if (guess == _hiddenword)
                    {
                        correctguess = true;
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.White;

                        Console.WriteLine("Congratulations! You guessed the word!");
                        Console.ResetColor();

                        DisplayAttempComment(attempts);
                        _score = maxAttempts - attempts;
                        Console.WriteLine($"\nYour score: {_score}");

                        break;
                    }
                }

                catch(InvalidGuessException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid guess: {ex.Message}");
                    Console.ResetColor();
                }
                catch(InvalidCastException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    attempts--;
                }

                Console.WriteLine();
            }

            if(!correctguess)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Game Over! The word was: {_hiddenword}");
                Console.ResetColor();

            }
     }

     //difficulty based attempt calculations

        public int getMaxAttempts(int difficulty)
        {
            switch (difficulty)
            {
                case 1:
                    return 6;
                case 2:
                    return 5;
                case 3:
                    return 4;
                default:
                    throw new InvalidCastException("Invalid difficulty level.");
            }
        }

        public void DisplayFeedback(string guess, string feedback)
        {
            Console.WriteLine();

            for(int i = 0 ; i< guess.Length; i++)
            {
                Console.Write(guess[i]+ " ");
            }
            Console.WriteLine();

            for(int i = 0; i< feedback.Length; i++)
            {
                if(feedback[i] == 'Y')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (feedback[i] == 'G')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Console.Write(feedback[i] + " ");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        public void DisplayAttempComment(int attempts)
        {
            Console.WriteLine();


            switch (attempts)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Genius!");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Excellent!");
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Great job!");
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("Good work!");
                    break;
                case 5:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Nice try!");
                    break;
                case 6:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("That was close!");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("No Comments!");
                    break;
            }
        }

    }
}