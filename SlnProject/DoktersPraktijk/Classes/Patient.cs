using Microsoft.Data.SqlClient;
using System.Data;

namespace DokterspraktijkLib;

// patient erft van persoon en voegt patiënt-specifieke eigenschappen toe
// bevat volledige CRUD (Create, Read, Update, Delete) methodes
public class Patient : Persoon
{
    // geslacht als enum (in db opgeslagen als int 0/1/2)
    public Geslacht Geslacht { get; set; }

    // geboortedatum (datetime in databank)
    public DateTime Geboortedatum { get; set; }

    // notificatie voorkeur als enum (in db opgeslagen als int 0-3)
    public Notificaties Notificaties { get; set; }

    // lege constructor
    public Patient() { }

    // volledige constructor
    public Patient(int id, string voornaam, string achternaam, Geslacht geslacht, string? gsm,
                   string email, string paswoord, DateTime geboortedatum, byte[]? profielFotoData,
                   Notificaties notificaties)
        : base(id, voornaam, achternaam, gsm, email, paswoord, profielFotoData)
    {
        Geslacht = geslacht;
        Geboortedatum = geboortedatum;
        Notificaties = notificaties;
    }

    // CRUD - read all: alle patie nten
    public static List<Patient> GeefAllePatienten()
    {
        List<Patient> patienten = new List<Patient>();
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient ORDER BY achternaam, voornaam";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    patienten.Add(LeesUitReader(reader));
                }
            }
        }
        return patienten;
    }

    // CRUD - read by email: voor inloggen
    public static Patient? GeefPatientPerEmail(string email)
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient WHERE email = @email";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
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
    public static Patient? GeefPatientPerId(int id)
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "SELECT id, voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties FROM Patient WHERE id = @id";
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

    // CRUD - create: voegt een nieuwe patient toe aan de databank
    // na het toevoegen wordt het nieuwe Id ingevuld in dit object
    public void Toevoegen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            // select scope_identity() haalt direct het auto-generated id op
            string sql = @"INSERT INTO Patient (voornaam, achternaam, geslacht, gsm, email, paswoord, geboortedatum, profielfotodata, notificaties)
                          VALUES (@voornaam, @achternaam, @geslacht, @gsm, @email, @paswoord, @geboortedatum, @profielfotodata, @notificaties);
                          SELECT CAST(SCOPE_IDENTITY() AS int);";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ZetParameters(cmd);
                // executeScalar geeft de eerste kolom van de eerste rij terug (= het nieuwe id)
                object? result = cmd.ExecuteScalar();
                if (result != null)
                {
                    this.Id = (int)result;
                }
            }
        }
    }

    // CRUD - update: werkt een bestaande patient bij in de databank
    public void Bijwerken()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = @"UPDATE Patient SET voornaam = @voornaam, achternaam = @achternaam, geslacht = @geslacht,
                          gsm = @gsm, email = @email, paswoord = @paswoord, geboortedatum = @geboortedatum,
                          profielfotodata = @profielfotodata, notificaties = @notificaties
                          WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ZetParameters(cmd);
                cmd.Parameters.AddWithValue("@id", this.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // CRUD - delete: verwijdert een patient uit de databank
    // verwijdert ook alle gekoppelde afspraken (zoals gevraagd in de opgave)
    public void Verwijderen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            // eerst de gekoppelde afspraken verwijderen om foreign-key conflicten te voorkomen
            string sqlAfspraken = "DELETE FROM Afspraak WHERE patient_id = @id";
            using (SqlCommand cmd1 = new SqlCommand(sqlAfspraken, conn))
            {
                cmd1.Parameters.AddWithValue("@id", this.Id);
                cmd1.ExecuteNonQuery();
            }

            // daarna de patient zelf verwijderen
            string sqlPatient = "DELETE FROM Patient WHERE id = @id";
            using (SqlCommand cmd2 = new SqlCommand(sqlPatient, conn))
            {
                cmd2.Parameters.AddWithValue("@id", this.Id);
                cmd2.ExecuteNonQuery();
            }
        }
    }

    // private helper-method om de SQL parameters in te vullen
    // wordt gebruikt door zowel toevoegen() als bijwerken()
    private void ZetParameters(SqlCommand cmd)
    {
        cmd.Parameters.AddWithValue("@voornaam", this.Voornaam);
        cmd.Parameters.AddWithValue("@achternaam", this.Achternaam);
        // (int) cast werkt voor enums omdat ze opgeslagen worden als int in db
        cmd.Parameters.AddWithValue("@geslacht", (int)this.Geslacht);
        // Null-check voor optionele velden
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
        cmd.Parameters.AddWithValue("@geboortedatum", this.Geboortedatum);
        if (this.ProfielFotoData == null)
        {
            cmd.Parameters.AddWithValue("@profielfotodata", DBNull.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@profielfotodata", this.ProfielFotoData);
        }
        cmd.Parameters.AddWithValue("@notificaties", (int)this.Notificaties);
    }

    // private helper om een DataReader rij om te zetten naar een Patient object
    private static Patient LeesUitReader(SqlDataReader reader)
    {
        Patient patient = new Patient();
        patient.Id = (int)reader["id"];
        patient.Voornaam = (string)reader["voornaam"];
        patient.Achternaam = (string)reader["achternaam"];
        patient.Geslacht = (Geslacht)(int)reader["geslacht"];
        if (reader["gsm"] != DBNull.Value)
        {
            patient.Gsm = ((string)reader["gsm"]).Trim();
        }
        patient.Email = (string)reader["email"];
        patient.Paswoord = (string)reader["paswoord"];
        patient.Geboortedatum = (DateTime)reader["geboortedatum"];
        if (reader["profielfotodata"] != DBNull.Value)
        {
            patient.ProfielFotoData = (byte[])reader["profielfotodata"];
        }
        patient.Notificaties = (Notificaties)(int)reader["notificaties"];
        return patient;
    }
}
