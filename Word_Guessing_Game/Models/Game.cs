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
        private readonly UserService _userService;

        private List<string> _guesses;
        private string _hiddenword;
        private int _score;

        public Game(UserService userService)
        {
            _userService = userService;
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
            Console.WriteLine("\n\n---------------------- Game Starts ----------------------");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine("Choose Difficulty");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");
            Console.WriteLine("4. Exit");

            Console.WriteLine("Enter choice: ");

            if (!int.TryParse(Console.ReadLine(), out int difficulty))
            {
                difficulty = 1;
            }
            if(difficulty == 4)
            {
                Console.WriteLine("Exiting the game. Goodbye!");
                return;
            }
            if (difficulty < 1 || difficulty > 3)
            {
                Console.WriteLine("Invalid difficulty level. Defaulting to Easy.");
                difficulty = 1;
            }

            int maxAttempts = GetMaxAttempts(difficulty);

            Console.WriteLine($"\nYou have {maxAttempts} attempts to guess the word.\n");

            bool correctguess = false;

            int attempts;
            for(attempts = 1; attempts <= maxAttempts; attempts++)
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Attempt {attempts}: Enter your guess: ");
                    string guess = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

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

                        DisplayAttemptComment(attempts);
                        _score = Math.Max(1, maxAttempts - attempts + 1);
                        string comment = GetAttemptComment(attempts);
                        Console.WriteLine($"\nYour score: {_score}");
                        Console.WriteLine($"Comment: {comment}");
                        _userService.SaveScore(_score, _hiddenword, attempts, comment);

                        break;
                    }
                }

                catch(InvalidGuessException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid guess: {ex.Message}");
                    Console.ResetColor();
                }
                catch(InvalidDifficultyException ex)
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

                _score = 0;
                Console.WriteLine($"Score: {_score}");
                _userService.SaveScore(_score, _hiddenword, maxAttempts, "No comment");

            }
     }

     //difficulty based attempt calculations

        public int GetMaxAttempts(int difficulty)
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
                    throw new InvalidDifficultyException("Invalid difficulty level.");
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

        public void DisplayAttemptComment(int attempts)
        {
            string comment = GetAttemptComment(attempts);
            Console.WriteLine();
            Console.ForegroundColor = GetCommentColor(attempts);
            Console.WriteLine(comment);
            Console.ResetColor();
        }

        private string GetAttemptComment(int attempts)
        {
            return attempts switch
            {
                1 => "Genius!",
                2 => "Excellent!",
                3 => "Great job!",
                4 => "Good work!",
                5 => "Nice try!",
                6 => "That was close!",
                _ => "No comment"
            };
        }

        private ConsoleColor GetCommentColor(int attempts)
        {
            return attempts switch
            {
                1 => ConsoleColor.DarkGreen,
                2 => ConsoleColor.Green,
                3 => ConsoleColor.Blue,
                4 => ConsoleColor.DarkYellow,
                5 => ConsoleColor.Yellow,
                6 => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };
        }

    }
}
