using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace CLImmo.Data
{
    public static class DatabaseInitializer
    {
        private static readonly string DbPad = Path.Combine(AppContext.BaseDirectory, "ImmoDB.db");

        public static string ConnectionString => $"Data Source={DbPad}";

        // Wordt eenmalig aangeroepen bij opstart van de applicatie.
        // Als het .db-bestand nog niet bestaat, wordt het aangemaakt aan de hand
        // van ImmoDB.sql (schema + testdata).
        public static void ZorgDatabankBestaat()
        {
            if (File.Exists(DbPad))
                return;

            var scriptPad = Path.Combine(AppContext.BaseDirectory, "ImmoDB.sql");

            if (!File.Exists(scriptPad))
            {
                throw new FileNotFoundException(
                    "ImmoDB.sql werd niet gevonden naast de executable. " +
                    "Controleer of het bestand de eigenschap 'Copy to Output Directory' " +
                    "op 'Copy if newer' heeft staan.",
                    scriptPad);
            }

            var script = File.ReadAllText(scriptPad);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = script;
            command.ExecuteNonQuery();
        }
    }
}
