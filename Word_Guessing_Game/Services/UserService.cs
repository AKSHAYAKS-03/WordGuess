using Npgsql;
using Word_Guessing_Game.Database;
using Word_Guessing_Game.Models;

namespace Word_Guessing_Game.Services
{
    public class UserService
    {
        public User? CurrentUser { get; private set; }

        public bool Authenticate()
        {
            while (CurrentUser is null)
            {
                Console.WriteLine("\n1. Login");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Exit");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Please enter a valid option.");
                    continue;
                }

                if (choice == 1 && Login())
                {
                    return true;
                }

                if (choice == 2 && Register())
                {
                    return true;
                }

                if (choice == 3)
                {
                    return false;
                }

                Console.WriteLine("Please choose a valid option.");
            }

            return true;
        }

        public bool Login()
        {
            Console.Write("Enter username: ");
            string username = (Console.ReadLine() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return false;
            }

            Console.Write("Enter password: ");
            string password = ReadPassword();

            using var conn = DbConnection.GetConnection();
            conn.Open();

            const string query = @"SELECT user_id, username, password, created_at FROM users WHERE username = @username LIMIT 1;";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username); // parameterized query to prevent SQL injection

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                Console.WriteLine("User not found. Please register first.");
                return false;
            }

            string storedPassword = reader.GetString(2);
            if (storedPassword != password)
            {
                Console.WriteLine("Invalid password.");
                return false;
            }

            CurrentUser = new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = storedPassword,
                CreatedAt = reader.GetDateTime(3)
            };

            Console.WriteLine($"Welcome back, {CurrentUser.Username}!");
            return true;
        }

        public bool Register()
        {
            Console.Write("Choose a username: ");
            string username = (Console.ReadLine() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return false;
            }

            Console.Write("Choose a password: ");
            string password = ReadPassword();

            using var conn = DbConnection.GetConnection();
            conn.Open();

            const string insertQuery = @" INSERT INTO users (username, password) VALUES (@username, @password) RETURNING user_id, created_at;";

            try
            {
                using var cmd = new NpgsqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                using var reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    Console.WriteLine("Registration failed.");
                    return false;
                }

                CurrentUser = new User
                {
                    Id = reader.GetInt32(0),
                    Username = username,
                    Password = password,
                    CreatedAt = reader.GetDateTime(1)
                };

                Console.WriteLine($"Registration successful. Welcome, {CurrentUser.Username}!");
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                Console.WriteLine("That username is already taken.");
                return false;
            }
        }

        public void DisplayScores()
        {
            if (CurrentUser is null)
            {
                Console.WriteLine("No user logged in.");
                return;
            }

            using var conn = DbConnection.GetConnection();
            conn.Open();

            const string query = @"SELECT u.username, s.score, s.hidden_word, s.attempts_used, s.comment, s.played_at FROM scores s INNER JOIN users u ON u.user_id = s.user_id WHERE s.user_id = @user_id ORDER BY s.played_at DESC LIMIT 10;";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@user_id", CurrentUser.Id);

            using var reader = cmd.ExecuteReader();
            Console.WriteLine("\nYour Recent Scores:");

            bool hasScores = false;
            while (reader.Read())
            {
                hasScores = true;
                string username = reader.GetString(0);
                int score = reader.GetInt32(1);
                string hiddenWord = reader.GetString(2);
                int attemptsUsed = reader.GetInt32(3);
                string comment = reader.GetString(4);
                DateTime date = reader.GetDateTime(5);

                Console.WriteLine($"Player: {username} | Score: {score} - Date: {date}");
                Console.WriteLine($"Word: {hiddenWord} - Attempts Used: {attemptsUsed} - Comment: {comment}");
                Console.WriteLine("--------------------------------------------------");
            }

            if (!hasScores)
            {
                Console.WriteLine("No scores found.");
            }
        }

        public void SaveScore(int score, string hiddenWord, int attemptsUsed, string comment)
        {
            if (CurrentUser is null)
            {
                return;
            }

            using var conn = DbConnection.GetConnection();
            conn.Open();

            const string insertQuery = @" INSERT INTO scores (user_id, hidden_word, attempts_used, score, comment) VALUES (@user_id, @hidden_word, @attempts_used, @score, @comment);";

            using var cmd = new NpgsqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@user_id", CurrentUser.Id);
            cmd.Parameters.AddWithValue("@hidden_word", hiddenWord);
            cmd.Parameters.AddWithValue("@attempts_used", attemptsUsed);
            cmd.Parameters.AddWithValue("@score", score);
            cmd.Parameters.AddWithValue("@comment", comment);
            cmd.ExecuteNonQuery();
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        private static string ReadPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[..^1];
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
