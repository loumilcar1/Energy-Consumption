using System;
using System.Threading.Tasks;

namespace ParserData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {

                // Imprimir solo la hora en formato HH:mm:ss
                Console.WriteLine("Start: " + DateTime.Now);
                Console.WriteLine("\n");

                // Inicializar la base de datos si no existe
                DatabaseInitializer dbInitializer = new DatabaseInitializer();
                dbInitializer.InitializeDatabase();

                Fetcher fetcher = new Fetcher();
                Parser parser = new Parser();
                DatabaseHandler databaseManager = new DatabaseHandler();

                // 1- Fetch data
                var (jsonSpain, jsonRegion) = await fetcher.FetchDataAsync();

                // Check if jsonSpain or jsonRegion are null
                if (jsonSpain != null || jsonRegion != null)
                {
                    // 2- Parse JSON data
                    var (dataSpain, dataRegion) = Parser.ParserData(jsonSpain, jsonRegion);

                    // 3- Insert data into database
                    await databaseManager.InsertDataAsync(dataSpain, dataRegion);
                    Console.WriteLine("\n");
                }
                else
                {
                    Console.WriteLine("No data fetched. Data is already up to date.");
                    Console.WriteLine("\n");
                }
                // Espera a que el usuario presione Enter para cerrar la consola
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
            }
        }
    }
}