namespace DokterspraktijkLib;

// Statische sessie-klasse om de ingelogde gebruiker te delen tussen pagina's
// Een van de twee properties is gevuld na succesvol inloggen, de andere blijft null
// Dit voorkomt dat we de gebruiker telkens moeten doorgeven aan elke pagina
public static class Sessie
{
    // De ingelogde dokter (null als geen dokter ingelogd is)
    public static Dokter? IngelogdeDokter { get; set; }

    // De ingelogde patiënt (null als geen patiënt ingelogd is)
    public static Patient? IngelogdePatient { get; set; }

    // Methode om uit te loggen - reset beide properties
    public static void Logout()
    {
        IngelogdeDokter = null;
        IngelogdePatient = null;
    }
}
