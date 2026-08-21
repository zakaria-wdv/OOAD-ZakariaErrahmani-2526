using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CLImmo.Models;

namespace CLImmo.Data
{
    public class PandRepository
    {
        public List<Pand> GetAllPanden()
        {
            var resultaat = new List<Pand>();

            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT id, adres, makelaarId, prijs, oppervlakte, energielabel,
                                            bouwjaar, type, tuinoppervlakte, heeftLift, foto,
                                            isVerkocht, datumVerkocht
                                     FROM panden";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var adres = reader.GetString(1);
                var makelaarId = reader.GetString(2);
                var prijs = reader.GetDouble(3);
                var oppervlakte = reader.GetInt32(4);
                var energielabel = Pand.ParseEnergielabel(reader.GetString(5));
                var bouwjaar = reader.GetInt32(6);
                var type = reader.GetString(7);
                int? tuin = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                bool? lift = reader.IsDBNull(9) ? null : reader.GetInt32(9) == 1;
                string? foto = reader.IsDBNull(10) ? null : reader.GetString(10);
                var isVerkocht = reader.GetInt32(11) == 1;
                DateTime? datumVerkocht = reader.IsDBNull(12) ? null : DateTime.Parse(reader.GetString(12));

                Pand pand = type == "Huis"
                    ? new Huis(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                               tuin ?? 0, foto, isVerkocht, datumVerkocht)
                    : new Appartement(adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar,
                                       lift ?? false, foto, isVerkocht, datumVerkocht);

                pand.Id = id;
                resultaat.Add(pand);
            }

            return resultaat;
        }

        // Voegt een nieuw pand toe en vult het Id-veld van het object aan
        // met het door SQLite toegekende autoincrement-id.
        public int VoegPandToe(Pand pand)
        {
            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"INSERT INTO panden
                (adres, makelaarId, prijs, oppervlakte, energielabel, bouwjaar, type,
                 tuinoppervlakte, heeftLift, foto, isVerkocht, datumVerkocht)
                VALUES (@adres, @makelaarId, @prijs, @oppervlakte, @energielabel, @bouwjaar, @type,
                        @tuin, @lift, @foto, @isVerkocht, @datumVerkocht);
                SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("@adres", pand.Adres);
            command.Parameters.AddWithValue("@makelaarId", pand.MakelaarId);
            command.Parameters.AddWithValue("@prijs", pand.Prijs);
            command.Parameters.AddWithValue("@oppervlakte", pand.Oppervlakte);
            command.Parameters.AddWithValue("@energielabel", pand.Energielabel.ToString());
            command.Parameters.AddWithValue("@bouwjaar", pand.Bouwjaar);
            command.Parameters.AddWithValue("@type", pand.Type);
            command.Parameters.AddWithValue("@tuin", pand is Huis huis ? huis.Tuinoppervlakte : (object)DBNull.Value);
            command.Parameters.AddWithValue("@lift", pand is Appartement app ? (app.HeeftLift ? 1 : 0) : (object)DBNull.Value);
            command.Parameters.AddWithValue("@foto", (object?)pand.Foto ?? DBNull.Value);
            command.Parameters.AddWithValue("@isVerkocht", pand.IsVerkocht ? 1 : 0);
            command.Parameters.AddWithValue("@datumVerkocht", (object?)pand.DatumVerkocht?.ToString("s") ?? DBNull.Value);

            var nieuwId = (long)command.ExecuteScalar()!;
            pand.Id = (int)nieuwId;
            return pand.Id;
        }

        // Markeert een pand als verkocht (vandaag als verkoopdatum)
        public void MarkeerVerkocht(int pandId)
        {
            using var connection = new SqliteConnection(DatabaseInitializer.ConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE panden SET isVerkocht = 1, datumVerkocht = @datum WHERE id = @id";
            command.Parameters.AddWithValue("@datum", DateTime.Now.ToString("s"));
            command.Parameters.AddWithValue("@id", pandId);
            command.ExecuteNonQuery();
        }
    }
}
