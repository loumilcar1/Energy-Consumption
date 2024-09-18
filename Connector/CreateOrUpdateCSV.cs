using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

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

        public DateTime? GetLastExportedDate()
        {
            var csvFiles = Directory.GetFiles(_filePath, "SpainData_*.csv");

            if (csvFiles.Length == 0)
            {
                // Si no hay archivos, devolver null, lo que implica que no se ha exportado nada aún
                return null;
            }

            // Ordenar los archivos por nombre (año y mes) para obtener el más reciente
            var latestCsvFile = csvFiles.OrderByDescending(f => f).FirstOrDefault();

            // Leer el archivo y obtener la última línea (última fecha registrada)
            var lastLine = File.ReadLines(latestCsvFile).LastOrDefault();

            if (lastLine != null)
            {
                var lastRecord = lastLine.Split(',');
                if (DateTime.TryParse(lastRecord[0], out DateTime lastDateTime))
                {
                    return lastDateTime;
                }
            }

            return null; // Si algo falla, devuelve null
        }

        // Método para actualizar o crear CSVs según los datos recibidos
        public void UpdateCsv(List<CSVData> records)
        {
            // Agrupar los datos por mes
            var groupedByMonth = records
                .GroupBy(r => new { r.DateTime.Year, r.DateTime.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

            foreach (var group in groupedByMonth)
            {
                string monthYear = $"{group.Key.Year}_{group.Key.Month:D2}";
                string filePath = Path.Combine(_filePath, $"SpainData_{monthYear}.csv");

                if (!File.Exists(filePath))
                {
                    // Si el archivo no existe, crearlo
                    using (StreamWriter writer = new StreamWriter(filePath, false))
                    {
                        writer.WriteLine("DateTime,Value");
                        foreach (var record in group)
                        {
                            writer.WriteLine($"{record.DateTime},{record.Value}");
                        }
                    }
                }
                else
                {
                    // Si el archivo ya existe, añadir los nuevos registros
                    AppendToCsv(filePath, group.ToList());
                }
            }
        }
        private void AppendToCsv(string filePath, List<CSVData> records)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var record in records)
                {
                    writer.WriteLine($"{record.DateTime},{record.Value}");
                }
            }
            Console.WriteLine($"Data successfully appended to CSV: {Path.GetFileName(filePath)}");
        }

        //REGION
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
