using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public abstract class Dier
    {
        // --- Properties (read-only: enkel een getter, waarde gezet in constructor) ---

        public int Id { get; }
        public string Naam { get; }
        public Eigenaar Eigenaar { get; }       // compositie: Dier bezit een Eigenaar-object
        public DateTime Geboortedatum { get; }
        public double Gewicht { get; }
        public Urgentie Urgentie { get; }
        public bool IsOpgenomen { get; }
        public DateTime? DatumOpgenomen { get; } // nullable: niet elk dier is opgenomen

        public static string DatabasePad { get; set; } = string.Empty;

        // --- Constructors ---

        // Volledige constructor: gebruikt bij het LADEN uit de databank (alle velden gekend)
        protected Dier(int id, string naam, Eigenaar eigenaar, DateTime geboortedatum,
                       double gewicht, Urgentie urgentie, bool isOpgenomen,
                       DateTime? datumOpgenomen)
        {
            Id = id;
            Naam = naam;
            Eigenaar = eigenaar;
            Geboortedatum = geboortedatum;
            Gewicht = gewicht;
            Urgentie = urgentie;
            IsOpgenomen = isOpgenomen;
            DatumOpgenomen = datumOpgenomen;
        }

        // Korte constructor: gebruikt bij het AANMAKEN van een nieuw dier (nog geen id,
        // nog niet opgenomen) — ketent via :this() naar de volledige constructor
        protected Dier(string naam, Eigenaar eigenaar, DateTime geboortedatum,
                       double gewicht, Urgentie urgentie)
            : this(0, naam, eigenaar, geboortedatum, gewicht, urgentie, false, null)
        {
        }

        // --- Methodes ---

        // Elke subklasse geeft zijn eigen gedetailleerde informatie terug
        public abstract string GeefInfo();

        // Gemeenschappelijke basisinfo; subklassen roepen dit aan vanuit hun GeefInfo()
        protected string GeefBasisInfo()
        {
            string opgenomenTekst = "Nee";
            if (IsOpgenomen && DatumOpgenomen.HasValue)
            {
                opgenomenTekst = "Ja  (" + DatumOpgenomen.Value.ToString("dd/MM/yyyy") + ")";
            }

            return "Naam:           " + Naam + Environment.NewLine +
                   "Geboortedatum:  " + Geboortedatum.ToString("dd/MM/yyyy") + Environment.NewLine +
                   "Gewicht:        " + Gewicht.ToString("0.##") + " kg" + Environment.NewLine +
                   "Urgentie:       " + Urgentie.ToString() + Environment.NewLine +
                   "Eigenaar:       " + Eigenaar.ToString() + Environment.NewLine +
                   "Opgenomen:      " + opgenomenTekst;
        }

        // Toont naam in de ListBox; "(Opgenomen)" zichtbaar als het dier opgenomen is
        public override string ToString()
        {
            if (IsOpgenomen)
            {
                return Naam + " (Opgenomen)";
            }
            return Naam;
        }

        // --- Databank: initialisatie ---

        // Maakt tabellen aan en vult ze met startdata als de databank nog leeg is.
        // Wordt één keer aangeroepen bij het opstarten van de WPF-applicatie.
        public static void InitialiseerDatabase()
        {
            using (SqliteConnection conn = new SqliteConnection("Data Source=" + DatabasePad))
            {
                conn.Open();

                SqliteCommand cmdEigenaars = conn.CreateCommand();
                cmdEigenaars.CommandText =
                    "CREATE TABLE IF NOT EXISTS eigenaars (" +
                    "    id TEXT PRIMARY KEY," +
                    "    voornaam TEXT NOT NULL," +
                    "    achternaam TEXT NOT NULL" +
                    ")";
                cmdEigenaars.ExecuteNonQuery();

                SqliteCommand cmdDieren = conn.CreateCommand();
                cmdDieren.CommandText =
                    "CREATE TABLE IF NOT EXISTS dieren (" +
                    "    id INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "    naam TEXT NOT NULL," +
                    "    eigenaarId TEXT NOT NULL REFERENCES eigenaars(id)," +
                    "    geboortedatum TEXT NOT NULL," +
                    "    gewicht REAL NOT NULL DEFAULT 0," +
                    "    urgentie TEXT NOT NULL DEFAULT 'Normaal'," +
                    "    type TEXT NOT NULL," +
                    "    ras TEXT," +
                    "    isGevaccineerd INTEGER," +
                    "    isOpgenomen INTEGER NOT NULL DEFAULT 0," +
                    "    datumOpgenomen TEXT" +
                    ")";
                cmdDieren.ExecuteNonQuery();

                // Seed-data enkel invoegen als de tabel nog leeg is
                SqliteCommand cmdTel = conn.CreateCommand();
                cmdTel.CommandText = "SELECT COUNT(*) FROM eigenaars";
                long aantalEigenaars = Convert.ToInt64(cmdTel.ExecuteScalar());

                if (aantalEigenaars == 0)
                {
                    SqliteCommand cmdSeedEigenaars = conn.CreateCommand();
                    cmdSeedEigenaars.CommandText =
                        "INSERT INTO eigenaars (id, voornaam, achternaam) VALUES " +
                        "('EP001', 'Emma',    'Peeters')," +
                        "('VB002', 'Lotte',   'Van den Berg')," +
                        "('DS003', 'Jonas',   'De Smet')," +
                        "('YB004', 'Yasmine', 'Bakir')," +
                        "('TC005', 'Thomas',  'Claes')";
                    cmdSeedEigenaars.ExecuteNonQuery();

                    SqliteCommand cmdSeedDieren = conn.CreateCommand();
                    cmdSeedDieren.CommandText =
                        "INSERT INTO dieren (naam, eigenaarId, geboortedatum, gewicht, urgentie, type, ras, isGevaccineerd, isOpgenomen, datumOpgenomen) VALUES " +
                        "('Bobbie',  'EP001', '2018-03-15', 12.5, 'Normaal', 'Hond', 'Labrador',         NULL, 0, NULL)," +
                        "('Nala',    'VB002', '2020-07-22',  4.2, 'Spoed',   'Kat',  NULL,               1,    1, '2025-06-10T09:30:00')," +
                        "('Max',     'DS003', '2016-11-08', 28.0, 'Spoed',   'Hond', 'Golden Retriever', NULL, 1, '2025-06-12T14:15:00')," +
                        "('Luna',    'YB004', '2021-04-30',  3.8, 'Normaal', 'Kat',  NULL,               0,    0, NULL)," +
                        "('Buddy',   'TC005', '2019-09-14', 10.3, 'Laag',    'Hond', 'Beagle',           NULL, 0, NULL)," +
                        "('Milo',    'EP001', '2022-01-18',  5.1, 'Normaal', 'Kat',  NULL,               1,    0, NULL)," +
                        "('Bella',   'DS003', '2017-06-25', 22.7, 'Spoed',   'Hond', 'Boxer',            NULL, 1, '2025-06-14T08:00:00')," +
                        "('Tiger',   'VB002', '2019-12-03',  4.9, 'Laag',    'Kat',  NULL,               0,    0, NULL)," +
                        "('Charlie', 'YB004', '2020-08-11',  6.8, 'Normaal', 'Hond', 'Poedel',           NULL, 0, NULL)," +
                        "('Lily',    'TC005', '2023-03-27',  3.2, 'Normaal', 'Kat',  NULL,               0,    0, NULL)";
                    cmdSeedDieren.ExecuteNonQuery();
                }
            }
        }
    }
}
