using Microsoft.Data.SqlClient;
using System.Data;

namespace DokterspraktijkLib;

// Afspraak klasse - vertegenwoordigt een rij in de Afspraak tabel
// Bevat CRUD methodes voor afspraken
public class Afspraak
{
    // Database ID
    public int Id { get; set; }

    // Datum en tijd van de afspraak
    public DateTime Moment { get; set; }

    // Reden voor de consultatie (in de databank heet dit veld 'klacht')
    public string Klacht { get; set; } = "";

    // Foreign keys naar de gekoppelde patiënt en dokter
    public int PatientId { get; set; }
    public int DokterId { get; set; }

    // Optionele referenties naar de gekoppelde objecten
    // Worden opgehaald wanneer we JOIN queries doen
    public Patient? Patient { get; set; }
    public Dokter? Dokter { get; set; }

    // Lege constructor
    public Afspraak() { }

    // Volledige constructor
    public Afspraak(int id, DateTime moment, string klacht, int patientId, int dokterId)
    {
        Id = id;
        Moment = moment;
        Klacht = klacht;
        PatientId = patientId;
        DokterId = dokterId;
    }

    // CRUD - READ BY DOKTER + DATUM: alle afspraken voor een dokter op een specifieke dag
    // Bevat ook de patient info via een JOIN (zodat we de naam kunnen tonen)
    public static List<Afspraak> GeefAfsprakenVoorDokterOpDag(int dokterId, DateTime datum)
    {
        List<Afspraak> afspraken = new List<Afspraak>();
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            // CAST naar date om alleen op de datum (niet uur) te filteren
            // We doen een JOIN met Patient om de naam direct mee op te halen
            string sql = @"SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id,
                                  p.voornaam AS p_voornaam, p.achternaam AS p_achternaam
                          FROM Afspraak a
                          INNER JOIN Patient p ON a.patient_id = p.id
                          WHERE a.dokter_id = @dokterId
                            AND CAST(a.moment AS date) = CAST(@datum AS date)
                          ORDER BY a.moment";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@dokterId", dokterId);
                cmd.Parameters.AddWithValue("@datum", datum.Date);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraak = new Afspraak();
                        afspraak.Id = (int)reader["id"];
                        afspraak.Moment = (DateTime)reader["moment"];
                        afspraak.Klacht = (string)reader["klacht"];
                        afspraak.PatientId = (int)reader["patient_id"];
                        afspraak.DokterId = (int)reader["dokter_id"];
                        // Maak een 'lichte' Patient aan met enkel naam
                        Patient p = new Patient();
                        p.Id = afspraak.PatientId;
                        p.Voornaam = (string)reader["p_voornaam"];
                        p.Achternaam = (string)reader["p_achternaam"];
                        afspraak.Patient = p;
                        afspraken.Add(afspraak);
                    }
                }
            }
        }
        return afspraken;
    }

    // CRUD - READ BY PATIENT: alle afspraken voor een patiënt (met dokter info)
    public static List<Afspraak> GeefAfsprakenVoorPatient(int patientId)
    {
        List<Afspraak> afspraken = new List<Afspraak>();
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = @"SELECT a.id, a.moment, a.klacht, a.patient_id, a.dokter_id,
                                  d.voornaam AS d_voornaam, d.achternaam AS d_achternaam
                          FROM Afspraak a
                          INNER JOIN Dokter d ON a.dokter_id = d.id
                          WHERE a.patient_id = @patientId
                          ORDER BY a.moment";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@patientId", patientId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Afspraak afspraak = new Afspraak();
                        afspraak.Id = (int)reader["id"];
                        afspraak.Moment = (DateTime)reader["moment"];
                        afspraak.Klacht = (string)reader["klacht"];
                        afspraak.PatientId = (int)reader["patient_id"];
                        afspraak.DokterId = (int)reader["dokter_id"];
                        Dokter d = new Dokter();
                        d.Id = afspraak.DokterId;
                        d.Voornaam = (string)reader["d_voornaam"];
                        d.Achternaam = (string)reader["d_achternaam"];
                        afspraak.Dokter = d;
                        afspraken.Add(afspraak);
                    }
                }
            }
        }
        return afspraken;
    }

    // CRUD - CREATE: nieuwe afspraak in de db
    public void Toevoegen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = @"INSERT INTO Afspraak (moment, klacht, patient_id, dokter_id)
                          VALUES (@moment, @klacht, @patient_id, @dokter_id);
                          SELECT CAST(SCOPE_IDENTITY() AS int);";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@moment", this.Moment);
                cmd.Parameters.AddWithValue("@klacht", this.Klacht);
                cmd.Parameters.AddWithValue("@patient_id", this.PatientId);
                cmd.Parameters.AddWithValue("@dokter_id", this.DokterId);
                object? result = cmd.ExecuteScalar();
                if (result != null)
                {
                    this.Id = (int)result;
                }
            }
        }
    }

    // CRUD - DELETE: verwijdert een afspraak uit de db
    public void Verwijderen()
    {
        using (SqlConnection conn = Database.GetConnection())
        {
            conn.Open();
            string sql = "DELETE FROM Afspraak WHERE id = @id";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", this.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
