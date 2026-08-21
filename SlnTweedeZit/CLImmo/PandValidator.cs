using System.Text.RegularExpressions;
using CLImmo.Models;

namespace CLImmo.Validation
{
    public class PandValidator
    {
        public int MinimumAantalTekensAdres { get; set; }

        // Standaardconstructor: minimum van 5 tekens
        public PandValidator() : this(5)
        {
        }

        // Constructor chaining: aangepast minimum aantal tekens
        public PandValidator(int minimumAantalTekensAdres)
        {
            MinimumAantalTekensAdres = minimumAantalTekensAdres;
        }

        // Adres bestaat enkel uit letters, cijfers, spaties en de leestekens , . -
        // en is minstens MinimumAantalTekensAdres lang
        public bool IsGeldigAdres(string adres)
        {
            if (string.IsNullOrWhiteSpace(adres))
                return false;

            if (adres.Length < MinimumAantalTekensAdres)
                return false;

            return Regex.IsMatch(adres, @"^[a-zA-Z0-9\s,.\-]+$");
        }

        // Prijs moet strikt groter zijn dan nul
        public bool IsGeldigePrijs(double prijs) => prijs > 0;

        // Combinatiemethode: te gebruiken vlak voordat een nieuw pand wordt toegevoegd
        public bool IsGeldigPand(Pand pand)
        {
            return IsGeldigAdres(pand.Adres) && IsGeldigePrijs(pand.Prijs);
        }
    }
}
