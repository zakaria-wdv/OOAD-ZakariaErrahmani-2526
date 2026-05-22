namespace DokterspraktijkLib;

// abstracte superklasse voor Dokter en Patient
// bevat alle gemeenschappelijke properties zoals besproken in de cursus (overerving)
// 'abstract' betekent dat je geen Persoon direct kan instantiëren,
// alleen via een subklasse Dokter of Patient
public abstract class Persoon
{
    // database ID (primary key) - 0 betekent nog niet opgeslagen in db
    public int Id { get; set; }

    // voornaam van de persoon
    public string Voornaam { get; set; } = "";

    // achternaam van de persoon
    public string Achternaam { get; set; } = "";

    // gsm nummer (mag null zijn in databank)
    public string? Gsm { get; set; }

    // email - wordt ook gebruikt om in te loggen
    public string Email { get; set; } = "";

    // het gehashte wachtwoord (SHA256)
    public string Paswoord { get; set; } = "";

    // de profielfoto als byte array (image kolom in databank)
    // mag null zijn als er nog geen foto is
    public byte[]? ProfielFotoData { get; set; }

    // lege constructor - nodig voor instantiatie zonder waarden
    protected Persoon() { }

    // constructor met basis-velden
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

    // helper-property die de volledige naam teruggeeft (handig voor UI)
    public string VolledigeNaam
    {
        get { return Voornaam + " " + Achternaam; }
    }
}
