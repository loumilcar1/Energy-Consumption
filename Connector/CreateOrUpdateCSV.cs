using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Collector
{
    class CreateOrUpdateCSV
    {
        private readonly string _filePath;

        public CreateOrUpdateCSV()
        {
            _filePath = ConfigurationManager.AppSettings["CSVFilePath"];
        }

        public void UpdateCsv(List<CSVData> records)
        {
            using (StreamWriter writer = new StreamWriter(_filePath, false)) // `false` para sobrescribir el archivo
            {
                writer.WriteLine("DateTime,Value"); // Encabezado del CSV

                foreach (var record in records)
                {
                    writer.WriteLine($"{record.DateTime},{record.Value}");
                }
            }

            Console.WriteLine("Data successfully exported to CSV.");
        }
    }
}
