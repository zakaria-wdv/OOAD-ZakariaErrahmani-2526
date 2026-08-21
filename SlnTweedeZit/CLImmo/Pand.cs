using System;
using CLImmo.Enums;

namespace CLImmo.Models
{
    // Abstract basisklasse: een "kaal" pand (zonder te weten of het een huis of
    // appartement is) mag nooit rechtstreeks aangemaakt worden.
    public abstract class Pand
    {
        // ---- Static ----
        // Houdt bij hoeveel Pand-objecten er in totaal zijn aangemaakt
        // (over alle afgeleide klassen heen).
        private static int aantalPanden = 0;
        public static int AantalPanden => aantalPanden;

        // Static helper: zet een string uit de databank (bv. "a", "C", " f ")
        // veilig om naar een Energielabel-enum, met een veilige fallback.
        public static Energielabel ParseEnergielabel(string waarde)
        {
            if (Enum.TryParse<Energielabel>(waarde?.Trim().ToUpper(), out var resultaat))
                return resultaat;

            return Energielabel.C;
        }

        // ---- Properties ----
        public int Id { get; set; }
        public string Adres { get; set; }
        public string MakelaarId { get; set; }
        public double Prijs { get; set; }
        public int Oppervlakte { get; set; }
        public Energielabel Energielabel { get; set; }
        public int Bouwjaar { get; set; }
        public bool IsVerkocht { get; set; }
        public DateTime? DatumVerkocht { get; set; }
        public string? Foto { get; set; }

        // Elke afgeleide klasse bepaalt zelf haar omschrijving ("Huis"/"Appartement")
        public abstract string Type { get; }

        // ---- Berekende eigenschappen ----
        public int Ouderdom => DateTime.Now.Year - Bouwjaar;

        public double PrijsPerVierkanteMeter =>
            Oppervlakte == 0 ? 0 : Math.Round(Prijs / Oppervlakte, 2);

        // ---- Constructors ----

        // Volledige constructor
        protected Pand(string adres, string makelaarId, double prijs, int oppervlakte,
                        Energielabel energielabel, int bouwjaar, string? foto,
                        bool isVerkocht = false, DateTime? datumVerkocht = null)
        {
            Adres = adres;
            MakelaarId = makelaarId;
            Prijs = prijs;
            Oppervlakte = oppervlakte;
            Energielabel = energielabel;
            Bouwjaar = bouwjaar;
            Foto = foto;
            IsVerkocht = isVerkocht;
            DatumVerkocht = datumVerkocht;

            aantalPanden++;
        }

        // Constructor chaining: pand zonder foto -> foto wordt null
        protected Pand(string adres, string makelaarId, double prijs, int oppervlakte,
                        Energielabel energielabel, int bouwjaar)
            : this(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar, null)
        {
        }

        // ---- Methodes ----

        // Virtual: geeft een basisimplementatie, afgeleide klassen breiden dit uit.
        // Wordt gebruikt op de woningkaart en in keuzelijsten.
        public virtual string GeefInfo()
        {
            string status = IsVerkocht
                ? $" [VERKOCHT op {DatumVerkocht:dd/MM/yyyy}]"
                : "";

            return $"{Type} - {Adres} | €{Prijs:N0} | {Oppervlakte} m² | " +
                   $"label {Energielabel} | bouwjaar {Bouwjaar} (ouderdom {Ouderdom} jaar) | " +
                   $"€{PrijsPerVierkanteMeter:N2}/m²{status}";
        }

        public override string ToString() => GeefInfo();
    }
}
