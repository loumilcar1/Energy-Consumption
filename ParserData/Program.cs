using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace ParserData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configura el planificador
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // Inicia el planificador
            await scheduler.Start();

            // Define el trabajo
            IJobDetail job = JobBuilder.Create<DataFetchJob>()
                .WithIdentity("dataFetchJob", "group1") // Identidad única del trabajo
                .Build();

            // Trigger para ejecutar inmediatamente al arrancar la aplicación
            ITrigger triggerNow = TriggerBuilder.Create()
                .WithIdentity("triggerNow", "group1")
                .StartNow()
                .Build();

            // Trigger para ejecutar todos los días a las 12:00 PM
            ITrigger triggerDaily = TriggerBuilder.Create()
                .WithIdentity("triggerDaily", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(13, 08))
                .ForJob(job) 
                .Build();

            // Programa el trabajo y los triggers en el planificador
            await scheduler.ScheduleJob(job, triggerNow);
            await scheduler.ScheduleJob(triggerDaily);

            // Espera a que el usuario presione Enter para cerrar la consola
            //Console.WriteLine("Presione Enter para salir...");
            Console.ReadLine();

            // Detén el planificador
            await scheduler.Shutdown();
        }
    }

    // Define el trabajo que se va a ejecutar
    public class DataFetchJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {

                // Imprimir solo la hora en formato HH:mm:ss
                Console.WriteLine("Start: " + DateTime.Now);
                Console.WriteLine("\n");
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
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
            }
        }
    }
}