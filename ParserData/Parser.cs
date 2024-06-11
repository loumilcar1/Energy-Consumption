using System;
using Newtonsoft.Json;

namespace ParserData
{
    public class Parser
    {
        public static (DateTime datetime, decimal value)[] ParserData(string jsonContent)
        {
            var data = JsonConvert.DeserializeObject<JsonData>(jsonContent);

            if (data?.Included == null)
            {
                throw new ArgumentNullException(nameof(data.Included), "The JSON does not contain any data");
            }

            var values = data.Included[0].Attributes.Values;
            var parsedData = new (DateTime datetime, decimal value)[values.Count];

            int index = 0;

            foreach (var item in values)
            {
                DateTime datetime = DateTime.Parse(item.Datetime);
                decimal value = item.value;
                parsedData[index++] = (datetime, value);
            }

            // Print parsed data
            Console.WriteLine("Date \t\t\t\t Value");
            foreach (var (datetime, value) in parsedData)
            {
                Console.WriteLine($"{datetime}\t\t{value}");
            }

            return parsedData;
        }
    }
}
