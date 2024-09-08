using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Connector
{
    class CreateOrUpdateCSV
    {
        private readonly string _filePath;
        private readonly string _regionFilePath;

        public CreateOrUpdateCSV()
        {
            _filePath = ConfigurationManager.AppSettings["CSVFilePath"];
            _regionFilePath = ConfigurationManager.AppSettings["CSVRegionFilePath"];
        }

        public void UpdateCsv(List<CSVData> records)
        {
            // Obtener el mes y el año actuales
            string currentMonthYear = DateTime.Now.ToString("yyyy_MM");

            // Determinar el archivo CSV actual
            string currentFilePath = Path.Combine(Path.GetDirectoryName(_filePath), $"SpainData_{currentMonthYear}.csv");

            // Si el archivo CSV no existe, lo crea; si existe, lo actualiza
            if (!File.Exists(currentFilePath))
            {
                using (StreamWriter writer = new StreamWriter(currentFilePath, false)) // `false` para sobrescribir el archivo
                {
                    writer.WriteLine("DateTime,Value"); // Encabezado del CSV

                    foreach (var record in records)
                    {
                        writer.WriteLine($"{record.DateTime},{record.Value}");
                    }
                }

                //Console.WriteLine($"Data successfully exported to new CSV: {Path.GetFileName(currentFilePath)}");
            }
            else
            {
                AppendToCsv(currentFilePath, records);
            }
        }

        private void AppendToCsv(string filePath, List<CSVData> records)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true)) // `true` para añadir al archivo existente
            {
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.DateTime},{record.Value}");
                }
            }
            Console.WriteLine($"Spain data updated successfully: {Path.GetFileName(filePath)}");
            Console.WriteLine("\n");
            Console.WriteLine($"Region data updated successfully: RegionData_2024_09.csv");

            //Console.WriteLine($"Data successfully appended to CSV: {Path.GetFileName(filePath)}");
        }

        public void UpdateCsvRegion(List<CSVDataRegion> records)
        {
            // Obtener el mes y el año actuales
            string currentMonthYear = DateTime.Now.ToString("yyyy_MM");

            // Determinar el archivo CSV actual
            string currentFilePath = Path.Combine(Path.GetDirectoryName(_regionFilePath), $"RegionData_{currentMonthYear}.csv");

            // Si el archivo CSV no existe, lo crea; si existe, lo actualiza
            if (!File.Exists(currentFilePath))
            {
                using (StreamWriter writer = new StreamWriter(currentFilePath, false)) // `false` para sobrescribir el archivo
                {
                    writer.WriteLine("IdRegion,DateTime,Value"); // Encabezado del CSV

                    foreach (var record in records)
                    {
                        writer.WriteLine($"{record.Id_Region},{record.DateTime},{record.Value}");
                    }
                }

                //Console.WriteLine($"Region data successfully exported to new CSV: {Path.GetFileName(currentFilePath)}");
                Console.WriteLine($"Region data updated successfully: {Path.GetFileName(currentFilePath)}");
            }
            else
            {
                AppendToCsvRegion(currentFilePath, records);
            }
        }

        private void AppendToCsvRegion(string filePath, List<CSVDataRegion> records)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true)) // `true` para añadir al archivo existente
            {
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.Id_Region},{record.DateTime},{record.Value}");
                }
            }

            //Console.WriteLine($"Region data successfully appended to CSV: {Path.GetFileName(filePath)}");
        }
    }
}
