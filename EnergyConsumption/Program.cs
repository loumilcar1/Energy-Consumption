using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyConsumption
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                //Parámetros configurables en App.config
                string geoId = ConfigurationManager.AppSettings["GeoIds"];
                string mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
                string mongoDatabaseName = ConfigurationManager.AppSettings["MongoDatabaseName"];
                string mongoCollectionName = ConfigurationManager.AppSettings["MongoCollectionName"];

                Parser parser = new Parser();
                Fetcher fetcher = new Fetcher();
                MongoDBHandler mongoDBHandler = new MongoDBHandler(mongoConnectionString, mongoDatabaseName, mongoCollectionName);

                // Obtener la comunidad
                string region = GetCommunity(geoId);

                if (region == "Region no encontrada")
                {
                    Console.WriteLine("Error: Region no encontrada.");
                    return;
                }

                // 1- Obtener datos de la API
                string data = await fetcher.FetchDataAsync();
                
                if (string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("Error al obtener los datos.");
                    return;
                }
                Console.WriteLine("Datos obtenidos:");
                Console.WriteLine(data);

                // 2- Parsear datos obtenidos
                List<(double Value, DateTime Datetime)> parsedData = ParseData(parser, data);

                //Imprimir datos parseados por consola
                Console.WriteLine($"Los datos obtenidos pertenecen a: {region}");
                
                foreach (var item in parsedData)
                {
                    Console.WriteLine($"Value: {item.Value}, Datetime: {item.Datetime}");
                }

                // 3- Almacenar datos en la base de datos de MongoDB
                await SaveDataToMongoDB(parsedData, region, mongoDBHandler);

                // Esperar la entrada del usuario para cerrar la consola
                Console.WriteLine("Presiona Enter para salir...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: {0}", e.Message);
            }
        }
        static string GetCommunity(string geoId)
        {
            Dictionary<string, string> communities = new Dictionary<string, string>()
            {
                { "4", "Andalucía" },
                { "5", "Aragón" },
                { "6", "Cantabria" },
                { "7", "Castilla la Mancha" },
                { "8", "Castilla y León" },
                { "9", "Cataluña" },
                { "10", "País Vasco" },
                { "11", "Principado de Asturias" },
                { "13", "Comunidad de Madrid" },
                { "14", "Comunidad de Navarra" },
                { "15", "Comunidad Valenciana" },
                { "16", "Extremadura" },
                { "17", "Galicia" },
                { "20", "La Rioja" },
                { "21", "Región de Murcia" }
            };

            return communities.ContainsKey(geoId) ? communities[geoId] : "Region no encontrada";
        }
        static List<(double Value, DateTime Datetime)> ParseData(Parser parser, string data)
        {
            try
            {
                List<(double Value, DateTime Datetime)> parsedData = parser.ParseData(data);
                if (parsedData == null || parsedData.Count == 0)
                {
                    Console.WriteLine("No se han encontrado datos parseados.");
                    return null;
                }

                return parsedData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing data: {0}", ex.Message);
                return null;
            }
        }
            static async Task SaveDataToMongoDB(List<(double Value, DateTime Datetime)> parsedData, string community, MongoDBHandler mongoDBHandler)
        {
            try
            {
                //Eliminar datos existentes en la colección
                await mongoDBHandler.ClearCollectionAsync();

                //Guardar datos
                await mongoDBHandler.SaveCommunityDataAsync(community, parsedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar los datos en MongoDB: {0}", ex.Message);
                throw;
            }
        }
    }
}