using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib;

// Statische helper class voor SHA256 wachtwoord hashing
// De wachtwoorden in de databank zijn met SHA256 gehashed (lowercase hex zonder dashes)
// Bron: Microsoft Docs - System.Security.Cryptography.SHA256
// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256
public static class Hasher
{
    // Hash een gewone tekst-string met SHA256 en geeft het resultaat terug als lowercase hex string
    public static string Hash(string tekst)
    {
        // SHA256.Create() retourneert een SHA256 hashing instance
        // De 'using' zorgt voor automatische Dispose() na gebruik
        using (SHA256 sha256 = SHA256.Create())
        {
            // Converteer de input string naar een byte array (UTF-8 encoding)
            byte[] bytes = Encoding.UTF8.GetBytes(tekst);

            // Bereken de hash - dit geeft 32 bytes terug voor SHA256
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // Bouw een hex string op uit de byte array
            // We gebruiken StringBuilder voor efficiëntie
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                // "x2" formatteert als 2-cijferige lowercase hexadecimal
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
