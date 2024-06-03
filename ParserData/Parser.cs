using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ParserData
{
    public class Parser
    {
        public List<(double Value, DateTime Datetime)> ParseData(string jsonData)
        {
            EnergyData energyData = JsonSerializer.Deserialize<EnergyData>(jsonData);

            List<(double Value, DateTime Datetime)> parsedData = new List<(double, DateTime)>();
            foreach (var included in energyData.Included)
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
