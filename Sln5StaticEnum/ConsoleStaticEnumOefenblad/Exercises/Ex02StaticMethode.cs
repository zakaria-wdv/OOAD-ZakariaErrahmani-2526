using ConsoleStaticEnumOefenblad.Exercises.Classes;

namespace ConsoleStaticEnumOefenblad.Exercises;

internal class Ex02StaticMethode
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: couponcodes controleren");
        Console.WriteLine("-------------");

        string[] codes = { "ABC12-DE", "AB12-DE", "XYZ99-BE" };
        Console.WriteLine("\ntesten IsGeldig() methode:\n");
        foreach (string code in codes)
        {
            Console.WriteLine($"Code {code} is {(CouponCode.ControleerCode(code) ? "geldig" : "ongeldig")}");
        }
        Console.WriteLine("\ntesten Beschrijf() methode:\n");
        foreach (string code in codes)
        {
            Console.WriteLine($"Code {code}: {CouponCode.Beschrijf(code)}");
        }

    }
}