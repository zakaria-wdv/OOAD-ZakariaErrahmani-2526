using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex03StaticClass
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 3: tekst analyseren");
        Console.WriteLine("-------------");

        Console.WriteLine();
        string[] teksten = new string[]
        {
            "dit is een test",
            "gratis truncate aanbieding",
            "Workshop C# basis",
            "delete titel",
            ""
        };
        foreach (string tekst in teksten)
        {
            int aantalWoorden = TekstAnalyse.AantalWoorden(tekst);
            bool bevatVerbodenWoord = TekstAnalyse.BevatVerbodenWoord(tekst);
            bool bevatVerbodenKarakter = TekstAnalyse.BevatVerbodenKarakter(tekst);
            bool isGeschiktVoorTitel = TekstAnalyse.IsGeschiktVoorTitel(tekst);
            Console.WriteLine($"'{tekst}':");
            Console.WriteLine($" - bevat {aantalWoorden} woorden");
            if (bevatVerbodenWoord) Console.WriteLine(" - bevat verboden woord");
            if (bevatVerbodenKarakter) Console.WriteLine(" - bevat verboden karakter");
            Console.WriteLine($" - is {(isGeschiktVoorTitel ? "geschikt" : "niet geschikt")} voor een titel");
        }
    }
}