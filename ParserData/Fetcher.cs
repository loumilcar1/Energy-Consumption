using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;

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

        public async Task<string> FetchDataAsync()
        {
            string url;

            if (useConfigurableUrl)
            {
                // Use configurable URL
                url = $"{baseUrl}?start_date={startDate}&end_date={endDate}&time_trunc={timeTrunc}&geo_limit={geoLimit}&geo_ids={geoIds}";
            }
            else
            {
                // Use URL for current day's data
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                url = $"{baseUrl}?start_date={today}T00:00&end_date={today}T23:59&time_trunc=day";
            }
           
            try
            {
                // Send GET request to the API
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Print the fetched data
                Console.WriteLine("Data fetched:");
                Console.WriteLine(responseBody);

                //// Check if the response body is empty, it is neccesary?
                if (string.IsNullOrEmpty(responseBody))
                {
                    Console.WriteLine("Error fetching data");
                }
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
                //return null // If you prefer not to throw the exception
            }
        }
    }
}
