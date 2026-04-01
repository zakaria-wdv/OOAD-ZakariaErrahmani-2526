using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex01StaticProperty
{
    public static void Run()
    {
        Console.WriteLine("Oefening 1: deelnemers registreren");
        Console.WriteLine("-------------");

        WorkshopDeelnemer d1 = new WorkshopDeelnemer("Amira", true);
        WorkshopDeelnemer d2 = new WorkshopDeelnemer("Bram", false);
        WorkshopDeelnemer d3 = new WorkshopDeelnemer("Noor", true);

        Console.WriteLine($"Aantal aangemaakt: {WorkshopDeelnemer.AantalAangemaakt}");
        Console.WriteLine($"Aantal aanwezig: {WorkshopDeelnemer.AantalAanwezig}");

        d3.ZetAfwezig();
        d3.ZetAfwezig();
        Console.WriteLine($"Aantal aanwezig na wijziging: {WorkshopDeelnemer.AantalAanwezig}");
    }
}