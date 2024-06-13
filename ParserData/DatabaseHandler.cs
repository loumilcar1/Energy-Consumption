using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace ParserData
{
    public class DatabaseHandler
    {
        private readonly string _connectionString;

        public DatabaseHandler()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EnergyConsumptionDB"].ConnectionString;
        }

        public async Task InsertDataAsync((DateTime datetime, decimal value)[] datos)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Eliminar datos existentes
                /*string deleteQuery = "DELETE FROM EnergyDemand_Spain";
                using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                {
                    await deleteCommand.ExecuteNonQueryAsync();
                }*/

                //Insertar datos nuevos
                foreach (var (datetime, value) in datos)
                {
                    string insertQuery = @"
                        IF NOT EXISTS (SELECT 1 FROM EnergyDemand_Spain WHERE datetime = @datetime)
                        BEGIN
                            INSERT INTO EnergyDemand_Spain(datetime, value)
                            VALUES (@datetime, @value)
                        END";

                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@datetime", datetime);
                        command.Parameters.AddWithValue("@value", value);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            // Data inserted
            Console.WriteLine("Data inserted into database.");
        }
    }
}
