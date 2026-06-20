using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public class Eigenaar
    {
        public string Id { get; }
        public string Voornaam { get; }
        public string Achternaam { get; }

        public static string DatabasePad { get; set; } = string.Empty;

        public Eigenaar(string id, string voornaam, string achternaam)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
        }

        public override string ToString()
        {
            return Voornaam + " " + Achternaam;
        }

        public static List<Eigenaar> LaadAlle()
        {
            List<Eigenaar> eigenaars = new List<Eigenaar>();

            using (SqliteConnection conn = new SqliteConnection("Data Source=" + DatabasePad))
            {
                conn.Open();
                SqliteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT id, voornaam, achternaam FROM eigenaars ORDER BY achternaam, voornaam";
                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    string voornaam = reader.GetString(1);
                    string achternaam = reader.GetString(2);
                    eigenaars.Add(new Eigenaar(id, voornaam, achternaam));
                }
            }

            return eigenaars;
        }
    }
}
