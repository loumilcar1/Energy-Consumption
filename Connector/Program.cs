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
using System.IO;

namespace Collector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-5QV54GB\\MSSQL_SERVER;Database=EnergyConsumption;Trusted_Connection=True;";
            string query = "SELECT fecha, demanda FROM Hola"; 

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    using (StreamWriter writer = new StreamWriter("C:/pruebas_TFG/data6.csv"))
                    {
                        // Escribir encabezados
                        writer.WriteLine("Fecha;Demanda");

                        // Escribir datos
                        while (reader.Read())
                        {
                            DateTime fecha = reader.GetDateTime(0);
                            decimal demanda = reader.GetDecimal(1);

                            // Formatear el número decimal con un punto en lugar de una coma
                            string demandaFormateada = demanda.ToString("0.0#####", CultureInfo.InvariantCulture);

                            writer.WriteLine($"{fecha};{demanda}");
                        }
                    }
                }

                Console.WriteLine("Datos exportados a datos.csv correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al exportar datos a CSV: " + ex.Message);
            }
        }
        /*var mongoDBHandler = new MongoDBHandler("mongodb://localhost:27017", "prueba", "prueba_1");
        var dataCSV = await mongoDBHandler.GetDataAsync();

        var updateCSV = new UpdateCSV("C:/pruebas_TFG/datos.csv");
        updateCSV.UpdateCsv(dataCSV);

        Console.WriteLine("CSV creado o actualizado exitosamente.");

        // Esperar la entrada del usuario para cerrar la consola
        Console.WriteLine("Presiona Enter para salir...");
        Console.ReadLine();*/
    }
}
