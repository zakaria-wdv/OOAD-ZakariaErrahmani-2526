using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CLImmo.Models;

namespace CLImmo.Data
{
    public class MakelaarRepository
    {
        public List<Makelaar> GetAllMakelaars()
        {
            var resultaat = new List<Makelaar>();

            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT id, voornaam, achternaam FROM makelaars";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultaat.Add(new Makelaar(reader.GetString(0), reader.GetString(1), reader.GetString(2)));
            }

            return resultaat;
        }
    }
}
