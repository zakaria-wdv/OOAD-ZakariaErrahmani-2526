using System;
using CLImmo.Enums;

namespace CLImmo.Models
{
    public class Appartement : Pand
    {
        public bool HeeftLift { get; set; }

        public override string Type => "Appartement";

        // Volledige constructor
        public Appartement(string adres, string makelaarId, double prijs, int oppervlakte,
                            Energielabel energielabel, int bouwjaar, bool heeftLift,
                            string? foto, bool isVerkocht = false, DateTime? datumVerkocht = null)
            : base(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                   foto, isVerkocht, datumVerkocht)
        {
            HeeftLift = heeftLift;
        }

        // Constructor chaining: appartement zonder foto
        public Appartement(string adres, string makelaarId, double prijs, int oppervlakte,
                            Energielabel energielabel, int bouwjaar, bool heeftLift)
            : this(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                   heeftLift, null)
        {
        }

        // Override: voegt lift-informatie toe aan de algemene info
        public override string GeefInfo()
        {
            string lift = HeeftLift ? "lift aanwezig" : "geen lift";
            return base.GeefInfo() + $" | {lift}";
        }
    }
}
