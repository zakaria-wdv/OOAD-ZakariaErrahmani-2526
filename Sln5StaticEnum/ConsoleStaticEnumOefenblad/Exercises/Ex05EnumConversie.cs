using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex05EnumConversie
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 5: enum conversies");
        Console.WriteLine("-------------");

        Console.WriteLine("\nOefening 5: enum conversies");
        Console.WriteLine("-------------");

        Prioriteit p1 = Prioriteit.Hoog;
        Console.WriteLine($"Enumwaarde: {p1}");

        int cijfer = (int)p1;
        Console.WriteLine($"Als int: {cijfer}");

        Prioriteit p2 = p1 + 1;
        Console.WriteLine($"Nog hogere prioriteit: {p2}");
    }
}