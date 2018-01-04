namespace BackTesting.Model.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using BackTesting.Model.MarketData;
    using CsvHelper;
    using CsvHelper.Configuration;

    public static class Csv2Frame
    {
        public static IDictionary<DateTime, Bar> LoadBarsFromFile(string filePath)
        {
            using (var textReader = new StreamReader(filePath))
            {
                return LoadFormTextReader(textReader);
            }
        }

        public static IDictionary<DateTime, Bar> LoadBarsFromString(string csvString)
        {
            using (var ms = new MemoryStream(Encoding.Default.GetBytes(csvString)))
            {
                using (var textReader = new StreamReader(ms))
                {
                    return LoadFormTextReader(textReader);
                }
            }
        }

        public static IDictionary<DateTime, Bar> LoadFormTextReader(TextReader reader)
        {
            var res = new Dictionary<DateTime, Bar>();
            var cfg = new Configuration();
            cfg.RegisterClassMap<BarCsvMap>();

            var csv = new CsvReader(reader, cfg);
            foreach (var bar in csv.GetRecords<Bar>())
            {
                if (res.ContainsKey((DateTime)bar.DateTime))
                {
                    res[(DateTime)bar.DateTime] = bar;
                }
                else
                {
                    res.Add((DateTime)bar.DateTime, bar);
                }
            }

            return res;
        }
    }
}
