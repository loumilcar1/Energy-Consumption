using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Connector
{
    class DatabaseHandler
    {
        private readonly string _connectionString;

        public DatabaseHandler()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EnergyConsumptionDB"].ConnectionString;
        }

        public async Task<List<CSVData>> FetchDataAsync()
        {
            string query = "SELECT datetime, value FROM EnergyDemand_Spain";
            List<CSVData> data = new List<CSVData>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                SqlCommand command = new SqlCommand(query, connection);
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
