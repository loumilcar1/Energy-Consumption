using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace ParserData
{
    public class DatabaseInitializer
    {
        private string _connectionString;
        private string _masterConnectionString;

        public DatabaseInitializer()
        {
            // Obtén la cadena de conexión de la configuración (App.config)
            _connectionString = ConfigurationManager.ConnectionStrings["EnergyConsumptionDB"].ConnectionString;

            // Usa la conexión al servidor "master" (sin base de datos específica)
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master" // Base de datos predeterminada para operaciones de sistema
            };
            _masterConnectionString = builder.ToString();
        }

        // Método principal para inicializar la base de datos
        public void InitializeDatabase()
        {
            if (!DatabaseExists("EnergyConsumption"))
            {
                Console.WriteLine("La base de datos no existe. Creando base de datos y tablas...");

                // Ejecutar script para crear la base de datos
                string createDbScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "EnergyConsumptionDB.sql");
                ExecuteSqlScript(_masterConnectionString, createDbScriptPath);

                // Ejecutar script para crear las tablas y rellenar datos
                string tablesScriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts", "TablesEnergyConsumption.sql");
                ExecuteSqlScript(_connectionString, tablesScriptPath);

                Console.WriteLine("Base de datos y tablas creadas correctamente.");
            }
            else
            {
                Console.WriteLine("La base de datos ya existe.");
            }
        }

        // Método para verificar si la base de datos existe
        private bool DatabaseExists(string databaseName)
        {
            using (SqlConnection connection = new SqlConnection(_masterConnectionString))
            {
                string query = $"SELECT database_id FROM sys.databases WHERE Name = @databaseName";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@databaseName", databaseName);
                    connection.Open();
                    var result = command.ExecuteScalar();
                    return result != null;
                }
            }
        }

        // Método para ejecutar un script SQL desde un archivo
        private void ExecuteSqlScript(string connectionString, string scriptPath)
        {
            if (File.Exists(scriptPath))
            {
                // Leer el contenido del archivo de script
                string script = File.ReadAllText(scriptPath);

                // Dividir el script en partes usando "GO" como separador
                // También eliminamos espacios en blanco antes y después del "GO" para mayor precisión
                string[] scriptParts = script.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    foreach (string scriptPart in scriptParts)
                    {
                        // Ejecutar cada parte del script
                        using (SqlCommand command = new SqlCommand(scriptPart, connection))
                        {
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error al ejecutar la siguiente parte del script: \n{scriptPart}\nError: {ex.Message}");
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"El archivo de script {scriptPath} no se encontró.");
            }
        }
    }
}

