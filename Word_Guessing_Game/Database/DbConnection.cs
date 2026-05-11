using System;
using Npgsql;

namespace Word_Guessing_Game.Database
{
    public static class DbConnection
    {
        private static string GetConnectionString()
        {
            return "Host=localhost;Port=5432;Database=gamedb;Username=postgres;Password=12345";
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(GetConnectionString());
        }

        public static void TestConnection()
        {
            using var conn = GetConnection();
            conn.Open();
            conn.Close();
        }
    }
}
