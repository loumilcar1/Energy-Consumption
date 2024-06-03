using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Configuration;

namespace EnergyConsumption
{
    class Fetcher
    {
        private static readonly HttpClient client = new HttpClient();

        //Parámetros de la URL configurables en App.config
        private readonly string baseUrl;
        private readonly string startDate;
        private readonly string endDate;
        private readonly string timeTrunc;
        private readonly string geoLimit;
        private readonly string geoIds;

        public Fetcher()
        {
            baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            startDate = ConfigurationManager.AppSettings["StartDate"];
            endDate = ConfigurationManager.AppSettings["EndDate"];
            timeTrunc = ConfigurationManager.AppSettings["TimeTrunc"];
            geoLimit = ConfigurationManager.AppSettings["GeoLimit"];
            geoIds = ConfigurationManager.AppSettings["GeoIds"];
        }

        public async Task<string> FetchDataAsync()
        {
            //URL completa
            string url = $"{baseUrl}?start_date={startDate}&end_date={endDate}&time_trunc={timeTrunc}&geo_limit={geoLimit}&geo_ids={geoIds}";
           
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }
        }
    }
}
