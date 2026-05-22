using System.Security.Cryptography;
using System.Text;

namespace DokterspraktijkLib;

// statische helper class voor SHA256 wachtwoord hashing
// de wachtwoorden in de databank zijn met SHA256 gehashed (lowercase hex zonder dashes)
// bron: Microsoft Docs - System.Security.Cryptography.SHA256
// https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256
public static class Hasher
{
    // hash een gewone tekst-string met SHA256 en geeft het resultaat terug als lowercase hex string
    public static string Hash(string tekst)
    {
        // SHA256.Create() retourneert een SHA256 hashing instance
        // de 'using' zorgt voor automatische Dispose() na gebruik
        using (SHA256 sha256 = SHA256.Create())
        {
            // converteer de input string naar een byte array (UTF-8 encoding)
            byte[] bytes = Encoding.UTF8.GetBytes(tekst);

            // bereken de hash - dit geeft 32 bytes terug voor SHA256
            byte[] hashBytes = sha256.ComputeHash(bytes);

            // bouw een hex string op uit de byte array
            // we gebruiken StringBuilder voor efficiëntie
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
