using System;
using CLImmo.Enums;

namespace CLImmo.Models
{
    public class Huis : Pand
    {
        public int Tuinoppervlakte { get; set; }

        public override string Type => "Huis";

        // Volledige constructor
        public Huis(string adres, string makelaarId, double prijs, int oppervlakte,
                    Energielabel energielabel, int bouwjaar, int tuinoppervlakte,
                    string? foto, bool isVerkocht = false, DateTime? datumVerkocht = null)
            : base(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                   foto, isVerkocht, datumVerkocht)
        {
            Tuinoppervlakte = tuinoppervlakte;
        }

        // Constructor chaining: huis zonder foto
        public Huis(string adres, string makelaarId, double prijs, int oppervlakte,
                    Energielabel energielabel, int bouwjaar, int tuinoppervlakte)
            : this(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                   tuinoppervlakte, null)
        {
        }

        // Override: voegt tuinoppervlakte toe aan de algemene info
        public override string GeefInfo()
        {
            return base.GeefInfo() + $" | tuin: {Tuinoppervlakte} m²";
        }
    }
}
