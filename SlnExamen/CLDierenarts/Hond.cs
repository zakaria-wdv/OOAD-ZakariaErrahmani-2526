namespace CLDierenarts
{
    public class Hond : Dier
    {
        public string Ras { get; }

        // Volledige constructor: gebruikt bij het LADEN uit de databank
        // :base() geeft de 8 gemeenschappelijke velden door aan Dier
        public Hond(int id, string naam, Eigenaar eigenaar, DateTime geboortedatum,
                    double gewicht, Urgentie urgentie, bool isOpgenomen,
                    DateTime? datumOpgenomen, string ras)
            : base(id, naam, eigenaar, geboortedatum, gewicht, urgentie, isOpgenomen, datumOpgenomen)
        {
            Ras = ras;
        }

        // Korte constructor: gebruikt bij het AANMAKEN via het formulier
        // :this() ketent naar de volledige constructor met standaardwaarden voor id/opgenomen
        public Hond(string naam, Eigenaar eigenaar, DateTime geboortedatum,
                    double gewicht, Urgentie urgentie, string ras)
            : this(0, naam, eigenaar, geboortedatum, gewicht, urgentie, false, null, ras)
        {
        }

        public override string GeefInfo()
        {
            return "Type:           Hond" + Environment.NewLine +
                   "Ras:            " + Ras + Environment.NewLine +
                   GeefBasisInfo();
        }
    }
}
