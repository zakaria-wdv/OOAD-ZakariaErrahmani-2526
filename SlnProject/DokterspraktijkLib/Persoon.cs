namespace DokterspraktijkLib;

// Abstracte superklasse voor Dokter en Patient
// Bevat alle gemeenschappelijke properties zoals besproken in de cursus (overerving)
// 'abstract' betekent dat je geen Persoon direct kan instantiëren,
// alleen via een subklasse Dokter of Patient
public abstract class Persoon
{
    // Database ID (primary key) - 0 betekent nog niet opgeslagen in db
    public int Id { get; set; }

    // Voornaam van de persoon
    public string Voornaam { get; set; } = "";

    // Achternaam van de persoon
    public string Achternaam { get; set; } = "";

    // GSM nummer (mag null zijn in databank)
    public string? Gsm { get; set; }

    // Email - wordt ook gebruikt om in te loggen
    public string Email { get; set; } = "";

    // Het gehashte wachtwoord (SHA256)
    public string Paswoord { get; set; } = "";

    // De profielfoto als byte array (image kolom in databank)
    // Mag null zijn als er nog geen foto is
    public byte[]? ProfielFotoData { get; set; }

    // Lege constructor - nodig voor instantiatie zonder waarden
    protected Persoon() { }

    // Constructor met basis-velden
    protected Persoon(int id, string voornaam, string achternaam, string? gsm, string email, string paswoord, byte[]? profielFotoData)
    {
        Id = id;
        Voornaam = voornaam;
        Achternaam = achternaam;
        Gsm = gsm;
        Email = email;
        Paswoord = paswoord;
        ProfielFotoData = profielFotoData;
    }

    // Helper-property die de volledige naam teruggeeft (handig voor UI)
    public string VolledigeNaam
    {
        get { return Voornaam + " " + Achternaam; }
    }
}
