namespace CLImmo.Models
{
    // Extra klasse: elk pand hoort bij een makelaar (via MakelaarId in Pand).
    public class Makelaar
    {
        public string Id { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }

        public Makelaar(string id, string voornaam, string achternaam)
        {
            Id = id;
            Voornaam = voornaam;
            Achternaam = achternaam;
        }

        public string GeefVolledigeNaam() => $"{Voornaam} {Achternaam}";

        public override string ToString() => GeefVolledigeNaam();
    }
}
