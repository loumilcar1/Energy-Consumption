using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using CsvHelper;
using CsvHelper.Configuration;
using ParserData;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Collector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-5QV54GB\\MSSQL_SERVER;Database=EnergyConsumption;Trusted_Connection=True;";
            string query = "SELECT fecha, demanda FROM Hola";
            string csvFilePath = "C:/pruebas_TFG/data6.csv";

            try
            {
                List<DataCSV> records = new List<DataCSV>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    int id = 0; // Dummy ID since you might not have it in your database

                    while (await reader.ReadAsync())
                    {
                        DateTime fecha = reader.GetDateTime(0);
                        decimal demanda = reader.GetDecimal(1);

                        records.Add(new DataCSV
                        {
                            Id = id++,
                            DateTime = fecha,
                            Value = demanda
                        });
                    }
                }

                UpdateCSV updateCSV = new UpdateCSV(csvFilePath);
                updateCSV.UpdateCsv(records);

                Console.WriteLine("Data successfully exported to CSV.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting data to CSV: " + ex.Message);
            }
        }
    }
}
