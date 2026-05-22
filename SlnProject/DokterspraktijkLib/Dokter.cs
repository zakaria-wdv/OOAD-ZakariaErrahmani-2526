using Microsoft.Data.SqlClient;
using System.Data;

namespace DokterspraktijkLib;

// Dokter erft van Persoon en voegt dokter-specifieke eigenschappen toe
// CRUD methodes voor de Dokter tabel zitten in deze class zelf
public class Dokter : Persoon
{
    // RIZIV nummer (uniek identificatienummer voor zorgverleners in België)
    public int Rizivnummer { get; set; }

    // Is de dokter geconventioneerd (volgt hij de officiële tarieven)
    // In de db is dit een tinyint (0 of 1), in C# is dit een bool
    public bool IsGeconventioneerd { get; set; }

    // Lege constructor
    public Dokter() { }

    // Volledige constructor
    public Dokter(int id, string voornaam, string achternaam, string? gsm, string email,
                  string paswoord, byte[]? profielFotoData, int rizivnummer, bool isGeconventioneerd)
        : base(id, voornaam, achternaam, gsm, email, paswoord, profielFotoData)
    {
        Rizivnummer = rizivnummer;
        IsGeconventioneerd = isGeconventioneerd;
    }

    // CRUD - READ ALL: haalt alle dokters uit de databank
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

    // CRUD - READ BY EMAIL: zoekt een dokter op basis van email (voor inloggen)
    // Retourneert null als er geen dokter gevonden is
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

    // CRUD - READ BY ID
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

    // CRUD - CREATE: voegt een nieuwe dokter toe aan de databank
    public void Toevoegen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = @"INSERT INTO Dokter (voornaam, achternaam, gsm, email, paswoord, profielfotodata, rizivnummer, isgeconventioneerd)
                          VALUES (@voornaam, @achternaam, @gsm, @email, @paswoord, @profielfotodata, @rizivnummer, @isgeconventioneerd);
                          SELECT CAST(SCOPE_IDENTITY() AS int);";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ZetParameters(cmd);
                object? result = cmd.ExecuteScalar();
                if (result != null)
                {
                    this.Id = (int)result;
                }
            }
        }
    }

    // CRUD - UPDATE: werkt een bestaande dokter bij in de databank
    public void Bijwerken()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = @"UPDATE Dokter SET voornaam = @voornaam, achternaam = @achternaam, gsm = @gsm,
                          email = @email, paswoord = @paswoord, profielfotodata = @profielfotodata,
                          rizivnummer = @rizivnummer, isgeconventioneerd = @isgeconventioneerd
                          WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ZetParameters(cmd);
                cmd.Parameters.AddWithValue("@id", this.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // CRUD - DELETE: verwijdert een dokter en zijn gekoppelde afspraken uit de databank
    public void Verwijderen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            // Eerst de gekoppelde afspraken verwijderen om foreign-key conflicten te voorkomen
            string sqlAfspraken = "DELETE FROM Afspraak WHERE dokter_id = @id";
            using (SqlCommand cmd1 = new SqlCommand(sqlAfspraken, conn))
            {
                cmd1.Parameters.AddWithValue("@id", this.Id);
                cmd1.ExecuteNonQuery();
            }

            // Daarna de dokter zelf verwijderen
            string sqlDokter = "DELETE FROM Dokter WHERE id = @id";
            using (SqlCommand cmd2 = new SqlCommand(sqlDokter, conn))
            {
                cmd2.Parameters.AddWithValue("@id", this.Id);
                cmd2.ExecuteNonQuery();
            }
        }
    }

    // Private helper om de SQL parameters in te vullen voor Toevoegen en Bijwerken
    private void ZetParameters(SqlCommand cmd)
    {
        cmd.Parameters.AddWithValue("@voornaam", this.Voornaam);
        cmd.Parameters.AddWithValue("@achternaam", this.Achternaam);
        if (this.Gsm == null)
        {
            cmd.Parameters.AddWithValue("@gsm", DBNull.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@gsm", this.Gsm);
        }
        cmd.Parameters.AddWithValue("@email", this.Email);
        cmd.Parameters.AddWithValue("@paswoord", this.Paswoord);
        if (this.ProfielFotoData == null)
        {
            cmd.Parameters.AddWithValue("@profielfotodata", DBNull.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@profielfotodata", this.ProfielFotoData);
        }
        cmd.Parameters.AddWithValue("@rizivnummer", this.Rizivnummer);
        // bool omzetten naar tinyint (0 of 1) voor de databank
        cmd.Parameters.AddWithValue("@isgeconventioneerd", this.IsGeconventioneerd ? 1 : 0);
    }

    // Private helper om een DataReader rij om te zetten naar een Dokter object
    // Voorkomt code-duplicatie tussen de verschillende READ methodes
    private static Dokter LeesUitReader(SqlDataReader reader)
    {
        Dokter dokter = new Dokter();
        dokter.Id = (int)reader["id"];
        dokter.Voornaam = (string)reader["voornaam"];
        dokter.Achternaam = (string)reader["achternaam"];
        // gsm kan NULL zijn in databank
        if (reader["gsm"] != DBNull.Value)
        {
            // Trim omdat het een nchar(10) is en padding spaties bevat
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
