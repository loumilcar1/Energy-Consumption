using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Collector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                DatabaseHandler databaseHandler = new DatabaseHandler();
                CreateOrUpdateCSV csvHandler = new CreateOrUpdateCSV();

                //Fetch data from database
                List<CSVData> records = await databaseHandler.FetchDataAsync();

                // Create or update the CSV file
                csvHandler.UpdateCsv(records);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting data to CSV: " + ex.Message);
            }
        }
    }
}
