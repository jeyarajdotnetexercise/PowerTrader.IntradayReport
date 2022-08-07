using CsvHelper;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Model;
using System.Globalization;
using System.Text;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services
{
    public class CSVReportService : ICSVReportService
    {
        private readonly ILogger<Worker> _logger;
        
        public CSVReportService( ILogger<Worker> logger)
        {
            _logger = logger;            
        }

        /// <summary>
        /// GenerateIntraReport helps to write the CSV file
        /// </summary>
        /// <returns></returns>
        public void GenerateIntraReport<T>(string path, List<T> intraReportModels)
        {
            //_logger.LogInformation("Open the CSV file for write operations", DateTimeOffset.Now);
            using (StreamWriter swInfraReport = new StreamWriter(path, false, new UTF8Encoding(true)))
            using (CsvWriter cwInfraReport = new CsvWriter(swInfraReport, CultureInfo.InvariantCulture))
            {
                cwInfraReport.WriteHeader<IntraReportView>();
                cwInfraReport.NextRecord();
                foreach (var report in intraReportModels)
                {
                    cwInfraReport.WriteRecord(report);
                    cwInfraReport.NextRecord();
                }
            }
            //_logger.LogInformation("CSV Intra report successfully generated", DateTimeOffset.Now);
        }      
    }
}
