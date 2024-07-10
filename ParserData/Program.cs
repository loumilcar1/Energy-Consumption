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
                Fetcher fetcher = new Fetcher();
                Parser parser = new Parser();
                DatabaseHandler databaseManager = new DatabaseHandler();

                // 1- Fetch data
                var (jsonSpain, jsonRegion) = await fetcher.FetchDataAsync();

                // Check if jsonSpain and jsonRegion are null
                if (jsonSpain != null && jsonRegion != null)
                {
                    // 2- Parse JSON data
                    var (dataSpain, dataRegion) = Parser.ParserData(jsonSpain, jsonRegion);

                    // 3- Insert data into database
                    await databaseManager.InsertDataAsync(dataSpain, dataRegion);
                }
                else
                {
                    Console.WriteLine("No data fetched. Data is already up to date. Skipping parsing and database insertion.");
                }

                // Wait for the user to press Enter to close the console
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
            }
        }
    }
}