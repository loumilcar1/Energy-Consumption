using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Collector
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
    }
}
