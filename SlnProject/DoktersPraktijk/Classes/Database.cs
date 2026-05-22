using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib;

// statische helper class voor database connectie
// centraliseert de connection string zodat op 1 plek beheerd hoeft wordt
public static class Database
{
    // connection string naar SQL Server Express LocalDB
    // pas dit aan indien je een andere SQL Server instance gebruikt
    public static string ConnectionString { get; set; } =
        @"Server=(localdb)\MSSQLLocalDB;Database=DokterspraktijkDB;Trusted_Connection=True;TrustServerCertificate=True;";

    // maakt een nieuwe SqlConnection met de geconfigureerde connection string
    // de caller is verantwoordelijk voor het sluiten/disposen van de connectie
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
