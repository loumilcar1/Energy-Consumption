using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;
using System.Collections.Generic;

namespace ParserData
{
    class Fetcher
    {
        private static readonly HttpClient client = new HttpClient();

        //Configurable URL parameters from App.config
        private readonly string baseUrl;
        private readonly string startDate;
        private readonly string endDate;
        private readonly string timeTrunc;
        private readonly string geoLimit;
        private readonly string geoIds;
        private readonly bool useConfigurableUrl;
        public Fetcher()
        {
            baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            startDate = ConfigurationManager.AppSettings["StartDate"];
            endDate = ConfigurationManager.AppSettings["EndDate"];
            timeTrunc = ConfigurationManager.AppSettings["TimeTrunc"];
            geoLimit = ConfigurationManager.AppSettings["GeoLimit"];
            geoIds = ConfigurationManager.AppSettings["GeoIds"];
            useConfigurableUrl = bool.Parse(ConfigurationManager.AppSettings["UseConfigurableUrl"]);
        }

        public async Task<(string, Dictionary<int, string>)> FetchDataAsync()
        {
            string urlSpain = null;
            Dictionary<int, string> responseRegions = new Dictionary<int, string>();

            if (useConfigurableUrl)
            {
                // Use configurable URL
                urlSpain = $"{baseUrl}?start_date={startDate}&end_date={endDate}&time_trunc={timeTrunc}&geo_limit={geoLimit}&geo_ids={geoIds}";
                responseRegions = null;
            }
            else
            {
                // Determine the start date based on the last date in the database
                DatabaseHandler dbSpainHandler = new DatabaseHandler();
                DatabaseHandler dbRegionHandler = new DatabaseHandler();
                DateTime lastDateSpainInDb = await dbSpainHandler.GetLastDateSpainAsync();
                DateTime lastDateRegionInDb = await dbRegionHandler.GetLastDateRegionAsync();
                DateTime today = DateTime.Today;

                // Construct urlSpain
                if (lastDateSpainInDb < today)
                {
                    DateTime startDateSpain = lastDateSpainInDb.AddDays(1);
                    urlSpain = $"{baseUrl}?start_date={startDateSpain:yyyy-MM-dd}T00:00&end_date={today:yyyy-MM-dd}T23:59&time_trunc=day";
                }

                if (lastDateRegionInDb.Month != today.Month || lastDateRegionInDb.Year != today.Year)
                {
                    DateTime startDateRegion = lastDateRegionInDb.AddDays(1);

                    // Prepare to collect responses for all regions
                    List<Task<(int, string)>> regionTasks = new List<Task<(int, string)>>();

                    // Iterate through each region and create its URL
                    foreach (var region in RegionConfigurations.Configurations)
                    {
                        string regionUrl = $"{baseUrl}?start_date={startDateRegion:yyyy-MM-dd}T00:00&end_date={today:yyyy-MM-dd}T23:59&time_trunc=month&geo_limit={region.Value.geoLimit}&geo_ids={region.Value.geoId}";

                        Console.WriteLine($"Fetching data for region {region.Key} with URL: {regionUrl}");

                        // Add the task to fetch region data
                        regionTasks.Add(FetchRegionDataAsync(region.Key, regionUrl));
                    }

                    // Await all region data fetches, handling exceptions for each task
                    var regionResults = await Task.WhenAll(regionTasks);

                    // Populate the responseRegions dictionary
                    foreach (var (regionId, regionJson) in regionResults)
                    {
                        if (!string.IsNullOrEmpty(regionJson))
                        {
                            responseRegions[regionId] = regionJson;
                        }
                    }
                }
            }
            if (urlSpain == null && responseRegions.Count == 0)
            {
                // No new data to fetch
                return (null, null);
            }

            try
            {
                string responseSpain = null;
                if (urlSpain != null)
                {
                    // Send GET request to the API for urlSpain
                    HttpResponseMessage response1 = await client.GetAsync(urlSpain);
                    response1.EnsureSuccessStatusCode();
                    responseSpain = await response1.Content.ReadAsStringAsync();

                    // Print the fetched data from urlSpain
                    Console.WriteLine("Data fetched from urlSpain:");
                    Console.WriteLine(responseSpain);
                }
                // Print the fetched data from urlRegion
                Console.WriteLine("Data fetched from urlRegion:");

                return (responseSpain, responseRegions);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }
        }

        private async Task<(int, string)> FetchRegionDataAsync(int regionId, string regionUrl)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(regionUrl);
                response.EnsureSuccessStatusCode();
                string regionData = await response.Content.ReadAsStringAsync();

                // Check if the response contains an error
                if (regionData.Contains("\"errors\""))
                {
                    Console.WriteLine($"No data for region {regionId}: {regionData}");
                    return (regionId, null); // Indicate no data by returning null
                }

                return (regionId, regionData);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nHttpRequestException Caught for region {regionId}!");
                Console.WriteLine("Message :{0} ", e.Message);
                return (regionId, null);
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nGeneral Exception Caught for region {regionId}!");
                Console.WriteLine("Message :{0} ", e.Message);
                return (regionId, null);
            }
        }
    }
}
