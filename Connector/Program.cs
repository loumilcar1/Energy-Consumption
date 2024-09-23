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
            // Configura el planificador
            StdSchedulerFactory factoryConnector = new StdSchedulerFactory();
            IScheduler schedulerConnector = await factoryConnector.GetScheduler();

            // Inicia el planificador
            await schedulerConnector.Start();

            // Define el trabajo
            IJobDetail jobConnector = JobBuilder.Create<DataExportJob>()
                .WithIdentity("dataExportJob", "group2") // Identidad única del trabajo
                .Build();

            // Trigger para ejecutar inmediatamente al arrancar la aplicación
            ITrigger triggerNowConnector = TriggerBuilder.Create()
                .WithIdentity("triggerNowConnector", "group2")
                .StartNow()
                .Build();

            // Trigger para ejecutar todos los días a las 12:00 PM
            ITrigger triggerDailyConnector = TriggerBuilder.Create()
                .WithIdentity("triggerDailyConnector", "group2")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(12, 57))
                .ForJob(jobConnector)
                .Build();

            // Programa el trabajo y los triggers en el planificador
            await schedulerConnector.ScheduleJob(jobConnector, triggerNowConnector);
            await schedulerConnector.ScheduleJob(triggerDailyConnector);

            // Espera a que el usuario presione Enter para cerrar la consola
            //Console.WriteLine("Presione Enter para salir...");
            Console.ReadLine();

            // Detén el planificador
            await schedulerConnector.Shutdown();

        }
    }
    public class DataExportJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
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
