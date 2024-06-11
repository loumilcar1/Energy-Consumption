using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ParserData;

namespace Collector
{
    class UpdateCSV
    {
        private readonly string _csvFilePath;

        public UpdateCSV(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        public void UpdateCsv(List<DataCSV> records)
        {
            bool fileExists = File.Exists(_csvFilePath);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = !fileExists
            };

            using (var writer = new StreamWriter(_csvFilePath, append: true))
            using (var csv = new CsvWriter(writer, config))
            {
                if (!fileExists)
                {
                    // Write the header if the file does not exist
                    csv.Context.RegisterClassMap<DataCSVMap>();
                    csv.WriteHeader<DataCSV>();
                    csv.NextRecord();
                }

                foreach (var record in records)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
            }
        }

        public class DataCSVMap : ClassMap<DataCSV>
        {
            public DataCSVMap()
            {
                Map(m => m.Id).Name("_id");
                Map(m => m.Value).Name("value");
                Map(m => m.DateTime).Name("datetime");
            }
        }
    }

    public class DataCSV
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public DateTime DateTime { get; set; }
    }
}
