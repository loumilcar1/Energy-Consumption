using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Connector
{
    class DatabaseHandler
    {
        private readonly string _connectionString;
        private readonly CreateOrUpdateCSV _csvHandler;

        public DatabaseHandler(CreateOrUpdateCSV csvHandler)
        {
            _csvHandler = csvHandler ?? throw new ArgumentNullException(nameof(csvHandler)); // Asegurarse de que no sea nulo
            _connectionString = ConfigurationManager.ConnectionStrings["EnergyConsumptionDB"].ConnectionString;
        }

        public async Task<List<CSVData>> FetchDataAsync()
        {
            // Obtener la última fecha exportada desde los CSV
            DateTime? lastExportedDate = _csvHandler.GetLastExportedDate();

            // Si no hay una última fecha exportada, tomaremos todos los datos
            string query = lastExportedDate.HasValue
                ? "SELECT datetime, value FROM EnergyDemand_Spain WHERE datetime > @LastExportedDate ORDER BY datetime"
                : "SELECT datetime, value FROM EnergyDemand_Spain ORDER BY datetime";

            List<CSVData> data = new List<CSVData>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);

                if (lastExportedDate.HasValue)
                {
                    command.Parameters.AddWithValue("@LastExportedDate", lastExportedDate.Value);
                }

                SqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    DateTime datetime = reader.GetDateTime(0);
                    decimal value = reader.GetDecimal(1);

                    data.Add(new CSVData
                    {
                        DateTime = datetime,
                        Value = value
                    });
                }
            }

            return data;
        }

        public async Task<List<CSVDataRegion>> FetchDataRegionAsync()
        {
            string query = "SELECT  datetime, value, id_region FROM EnergyDemand_Region";
            List<CSVDataRegion> data = new List<CSVDataRegion>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        DateTime dateTime = reader.GetDateTime(reader.GetOrdinal("datetime"));
                        decimal value = reader.GetDecimal(reader.GetOrdinal("value"));
                        int idRegion = reader.GetInt32(reader.GetOrdinal("id_region"));

                        data.Add(new CSVDataRegion
                        {
                            DateTime = dateTime,
                            Value = value,
                            Id_Region = idRegion
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("General Error: " + ex.Message);
            }

            return data;
        }

    }

}
