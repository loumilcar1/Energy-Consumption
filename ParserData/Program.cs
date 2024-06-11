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
                DatabaseHandler baseDatosManager = new DatabaseHandler();

                // 1- Fetch data
                string jsonContent = await fetcher.FetchDataAsync();

                // 2- Parse JSON data
                var data = Parser.ParserData(jsonContent);

                // 3- Insert data into database
                await baseDatosManager.InsertDataAsync(data);

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