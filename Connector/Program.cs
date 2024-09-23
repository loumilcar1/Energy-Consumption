using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Connector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                // Imprimir solo la hora en formato HH:mm:ss
                Console.WriteLine("Start: " + DateTime.Now);

                // Crear la instancia de CreateOrUpdateCSV
                CreateOrUpdateCSV csvHandler = new CreateOrUpdateCSV();

                // Pasar esa instancia a DatabaseHandler
                DatabaseHandler databaseHandler = new DatabaseHandler(csvHandler);

                // Fetch data from EnergyDemand_Spain
                List<CSVData> records = await databaseHandler.FetchDataAsync();

                // Fetch data from EnergyConsumption_Region
                List<CSVDataRegion> regionRecords = await databaseHandler.FetchDataRegionAsync();


                if (records.Any())
                {

                    // Create or update the CSV file for EnergyDemand_Spain
                    csvHandler.UpdateCsv(records);

                }
                else
                {
                    Console.WriteLine("\nNo new data to export for EnergyDemand_Spain.");
                }


                if (regionRecords.Any())
                {
                    // Create or update the CSV file for EnergyConsumption_Region
                    csvHandler.UpdateCsvRegion(regionRecords);
                }
                else
                {
                    Console.WriteLine("\nNo new data to export for EnergyConsumption_Region.");
                }

                // Espera a que el usuario presione Enter para cerrar la consola
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting data to CSV: " + ex.Message);
            }
        }
    }
}
