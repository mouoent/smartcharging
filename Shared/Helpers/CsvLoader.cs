using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Shared.Models;

namespace Shared.Utilities
{
    public static class CsvLoader
    {
        public static IEnumerable<T> LoadCsvData<T>(string filePath, ClassMap<T> classMap) where T : BaseEntity
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            // If a custom class map is provided, use it
            if (classMap != null)
            {
                csv.Context.RegisterClassMap(classMap);
            }

            return csv.GetRecords<T>().ToList();
        }
    }
}
