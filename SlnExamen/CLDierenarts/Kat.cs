namespace CLDierenarts
{
    public class Kat : Dier
    {
        public bool IsGevaccineerd { get; }

        // Volledige constructor: gebruikt bij het LADEN uit de databank
        // :base() geeft de 8 gemeenschappelijke velden door aan Dier
        public Kat(int id, string naam, Eigenaar eigenaar, DateTime geboortedatum,
                   double gewicht, Urgentie urgentie, bool isOpgenomen,
                   DateTime? datumOpgenomen, bool isGevaccineerd)
            : base(id, naam, eigenaar, geboortedatum, gewicht, urgentie, isOpgenomen, datumOpgenomen)
        {
            IsGevaccineerd = isGevaccineerd;
        }

        // Korte constructor: gebruikt bij het AANMAKEN via het formulier
        // :this() ketent naar de volledige constructor met standaardwaarden voor id/opgenomen
        public Kat(string naam, Eigenaar eigenaar, DateTime geboortedatum,
                   double gewicht, Urgentie urgentie, bool isGevaccineerd)
            : this(0, naam, eigenaar, geboortedatum, gewicht, urgentie, false, null, isGevaccineerd)
        {
        }

        public override string GeefInfo()
        {
            string gevaccineerdTekst = "Nee";
            if (IsGevaccineerd)
            {
                gevaccineerdTekst = "Ja";
            }

            return "Type:           Kat" + Environment.NewLine +
                   "Gevaccineerd:   " + gevaccineerdTekst + Environment.NewLine +
                   GeefBasisInfo();
        }
    }
}
