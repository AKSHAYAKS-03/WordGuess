using Word_Guessing_Game.Database;
using Word_Guessing_Game.Models;
using Word_Guessing_Game.Services;

namespace Word_Guessing_Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DbConnection.TestConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database connection failed.");
                Console.WriteLine(ex.Message);
                return;
            }

            var userService = new UserService();

            Console.WriteLine("=========================== WORD GUESSING GAME ============================");
            Console.WriteLine("Play starts only after login or registration.");

            if (!userService.Authenticate())
            {
                Console.WriteLine("Exiting the game. Goodbye!");
                return;
            }

            while (true)
            {
                if (userService.CurrentUser is null)
                {
                    if (!userService.Authenticate())
                    {
                        Console.WriteLine("Exiting the game. Goodbye!");
                        return;
                    }
                }

                Console.WriteLine($"\nLogged in as {userService.CurrentUser!.Username}");
                Console.WriteLine("1. Start Game");
                Console.WriteLine("2. View Scores");
                Console.WriteLine("3. Logout");
                Console.WriteLine("4. Exit");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Please choose a valid option.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                    {
                        bool replay;
                        do
                        {
                            var game = new Game(userService);
                            game.StartGame();

                            Console.Write("Play again? (Y/N): ");
                            replay = string.Equals(Console.ReadLine()?.Trim(), "Y", StringComparison.OrdinalIgnoreCase);
                        } while (replay);

                        break;
                    }
                    case 2:
                        userService.DisplayScores();
                        break;
                    case 3:
                        userService.Logout();
                        Console.WriteLine("Logged out successfully.");
                        if (!userService.Authenticate())
                        {
                            Console.WriteLine("Exiting the game. Goodbye!");
                            return;
                        }
                        break;
                    case 4:
                        Console.WriteLine("Exiting the game. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Please choose a valid option.");
                        break;
                }
            }
        }
    }
}
