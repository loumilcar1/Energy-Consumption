using System;
using System.Collections.Generic;
using System.Text.Json;

namespace EnergyConsumption
{
    public class Parser
    {
        public List<(double Value, DateTime Datetime)> ParseData(string jsonData)
        {
            Root root = JsonSerializer.Deserialize<Root>(jsonData);

            List<(double Value, DateTime Datetime)> parsedData = new List<(double, DateTime)>();
            foreach (var included in root.Included)
            {
                if (included.Attributes?.Values != null)
                {
                    foreach (var value in included.Attributes.Values)
                    {
                        parsedData.Add((value.ValueAmount, DateTime.Parse(value.Datetime)));
                    }
                }
            }

            return parsedData;
        }
    }
}
