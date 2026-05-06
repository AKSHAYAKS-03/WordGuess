using Word_Guessing_Game.Models;

namespace Word_Guessing_Game
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool Replay = true;

            while(Replay)
            {
                Game game = new Game();
                game.StartGame();

                Console.WriteLine("Do you want to play again? (Y/N)");
                string choice = (Console.ReadLine() ?? "N").ToUpper();

                if(choice != "Y")
                {
                    Replay = false;
                }

            }
            Console.WriteLine("Thanks for playing!");
            
        }
    }
}