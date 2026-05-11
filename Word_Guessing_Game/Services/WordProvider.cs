using Word_Guessing_Game.Database;
using Npgsql;

namespace Word_Guessing_Game.Services
{
    public class WordProvider
    {
        public string GetRandomWord()
        {
            using var conn = DbConnection.GetConnection();
            conn.Open();

            const string query = "SELECT word FROM words ORDER BY RANDOM() LIMIT 1;";
            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetString(0).ToUpperInvariant();
            }

            return "APPLE";
        }
    }
}
