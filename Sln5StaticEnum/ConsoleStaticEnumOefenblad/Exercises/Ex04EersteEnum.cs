using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex04EersteEnum
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 4: bestelstatus");
        Console.WriteLine("-------------");

        Console.WriteLine("\nOefening 4: bestelstatus");
        Console.WriteLine("-------------");

        Bestelling b1 = new Bestelling
        {
            KlantNaam = "Sara",
            ProductNaam = "Laptop",
            Status = BestelStatus.Nieuw
        };
        Bestelling b2 = new Bestelling
        {
            KlantNaam = "Imran",
            ProductNaam = "Muis",
            Status = BestelStatus.Verzonden
        };
        Console.WriteLine(b1);
        Console.WriteLine(b2);
    }
}