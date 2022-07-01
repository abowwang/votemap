using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace VoteMap.Core.Utility
{
    public interface ICSVUtility
    {
        List<T> getDataFromCSV<T>(String filePath) where T : new();
        void writeDataToCSV<T>(List<T> input, String output);
    }

    public class CSVUtility : ICSVUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region  Get Data From CSV
        public List<T> getDataFromCSV<T>(String filePath) where T : new()
        {
            List<T> resut = new List<T>();
            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    IEnumerable<T> records = csv.GetRecords<T>();
                    resut = records.ToList();
                }
            }
            return resut;
        }
        #endregion

        #region Write Data To CSV
        public void writeDataToCSV<T>(List<T> input, String output)
        {
            logger.Info($"Write Data To CSV - start - {output}");

            if (input.Count <= 0)
            {
                logger.Info("no record");
            }
            else
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    ShouldQuote = args => true
                };

                using (var writer = new StreamWriter(output))
                {
                    using (var csv = new CsvWriter(writer, config))
                    {
                        csv.WriteRecords(input);
                    }
                }
            }
            logger.Info($"Write Data To CSV - end ");

        }
        #endregion
    }
}
