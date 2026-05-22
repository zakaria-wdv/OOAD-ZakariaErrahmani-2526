namespace DokterspraktijkLib;

// statische sessie-klasse om de ingelogde gebruiker te delen tussen pagina's
// een van de twee properties is gevuld na succesvol inloggen, de andere blijft null
// dit voorkomt dat we de gebruiker telkens moeten doorgeven aan elke pagina
public static class Sessie
{
    // de ingelogde dokter (null als geen dokter ingelogd is)
    public static Dokter? IngelogdeDokter { get; set; }

    // de ingelogde patiënt (null als geen patiënt ingelogd is)
    public static Patient? IngelogdePatient { get; set; }

    // methode om uit te loggen - reset beide properties
    public static void Logout()
    {
        IngelogdeDokter = null;
        IngelogdePatient = null;
    }
}
