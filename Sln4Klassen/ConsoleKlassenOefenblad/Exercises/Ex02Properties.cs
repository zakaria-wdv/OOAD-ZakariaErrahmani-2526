using ConsoleKlassenOefenblad.Exercises.Classes;

namespace ConsoleKlassenOefenblad.Exercises;

internal class Ex02Properties
{
    public static void Run()
    {
        Console.WriteLine("\nOefening 2: properties, standaardwaarden, object initializer syntax");
        Console.WriteLine("-------------");
        // 1. maak in "Exercises/Classes" een klasse "Recept" met volgende properties:
        //   - Titel
        //   - Rating (int)
        //   - IsVegetarisch (standaardwaarde is false)
        //   - Ingredienten (List van strings, standaard lege lijst)

        // 2. maak volgend recept aan met de lege constructor (... = new Recept()) en stel dan één voor één de properties in:
        //   - Pasta Carbonara (Rating 4, Ingrediënten: Pasta, Eieren, Spek, Parmezaanse kaas)
        // ...

        // 3. maak volgende recepten aan met de object initializer syntax:
        //   - Lasagne (Rating 5, IsVegetarisch false, Ingrediënten: Lasagnebladen, Tomatensaus, Courgette, Aubergine, Mozzarella)
        //   - Salade Niçoise (Rating 4, IsVegetarisch true, Ingrediënten: Sla, Tonijn, Eieren, Pindakaas, Olijven, Tomaten)
        // ...

        // 4. pas het recept van de salade niçoise aan:
        //  - verwijder de pindakaas
        //  - zet IsVegetarisch op false
        // ...

        // 5. maak een lijst "kookboek" aan en voeg de drie recepten toe
        // ...

        // 6. toon het aantal vegetarische recepten (zie screenshot) en de gemiddelde rating
        // ...
    }
}
