using Microsoft.Data.SqlClient;
using System.Data;

namespace DokterspraktijkLib;

// dokter erft van persoon en voegt dokter-specifieke eigenschappen toe
// CRUD methodes voor de Dokter tabel zitten in deze class zelf
public class Dokter : Persoon
{
    // RIZIV nummer (uniek identificatienummer voor zorgverleners in België)
    public int Rizivnummer { get; set; }

    // is de dokter geconventioneerd (volgt hij de officiële tarieven)
    // in de db is dit tinyint (0 of 1), in C# is dit een bool
    public bool IsGeconventioneerd { get; set; }

    // lege constructor
    public Dokter() { }

    // volledige constructor
    public Dokter(int id, string voornaam, string achternaam, string? gsm, string email,
                  string paswoord, byte[]? profielFotoData, int rizivnummer, bool isGeconventioneerd)
        : base(id, voornaam, achternaam, gsm, email, paswoord, profielFotoData)
    {
        Rizivnummer = rizivnummer;
        IsGeconventioneerd = isGeconventioneerd;
    }

    // CRUD - read all: haalt alle dokters uit de databank
    public static List<Dokter> GeefAlleDokters()
    {
        List<Dokter> dokters = new List<Dokter>();
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter ORDER BY achternaam, voornaam";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    dokters.Add(LeesUitReader(reader));
                }
            }
        }
        return dokters;
    }

    // CRUD - read by email: zoekt een dokter op basis van email (voor inloggen)
    // retourneert null als er geen dokter gevonden is
    public static Dokter? GeefDokterPerEmail(string email)
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter WHERE email = @email";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // Parametrische query - voorkomt SQL injection
                cmd.Parameters.AddWithValue("@email", email);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeesUitReader(reader);
                    }
                }
            }
        }
        return null;
    }

    // CRUD - read by id
    public static Dokter? GeefDokterPerId(int id)
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd FROM Dokter WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return LeesUitReader(reader);
                    }
                }
            }
        }
        return null;
    }

    // private helper method om een DataReader rij om te zetten naar een Dokter object
    // voorkomt code duplicatie tussen de verschillende read     methodes
    private static Dokter LeesUitReader(SqlDataReader reader)
    {
        Dokter dokter = new Dokter();
        dokter.Id = (int)reader["id"];
        dokter.Voornaam = (string)reader["voornaam"];
        dokter.Achternaam = (string)reader["achternaam"];
        // gsm kan NULL zijn in databank
        if (reader["gsm"] != DBNull.Value)
        {
            // trim omdat het een nchar(10) is en padding spaties bevat
            dokter.Gsm = ((string)reader["gsm"]).Trim();
        }
        dokter.Email = (string)reader["email"];
        dokter.Paswoord = (string)reader["paswoord"];
        // profielfotodata kan NULL zijn
        if (reader["profielfotodata"] != DBNull.Value)
        {
            dokter.ProfielFotoData = (byte[])reader["profielfotodata"];
        }
        dokter.Rizivnummer = (int)reader["rizivnummer"];
        // tinyint wordt omgezet naar bool
        dokter.IsGeconventioneerd = ((byte)reader["isgeconventioneerd"]) == 1;
        return dokter;
    }
}
