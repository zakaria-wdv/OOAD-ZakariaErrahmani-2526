using Microsoft.Data.SqlClient;

namespace DokterspraktijkLib;

// Statische helper class voor de database connectie
// Centraliseert de connection string zodat deze maar op één plek beheerd hoeft te worden
public static class Database
{
    // Connection string naar SQL Server Express LocalDB
    // Pas dit aan indien je een andere SQL Server instance gebruikt
    public static string ConnectionString { get; set; } =
        @"Server=zakaria\sqlexpress;Database=DokterspraktijkDB;Trusted_Connection=True;TrustServerCertificate=True;";

    // Maakt een nieuwe SqlConnection met de geconfigureerde connection string
    // De caller is verantwoordelijk voor het sluiten/disposen van de connectie
    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
