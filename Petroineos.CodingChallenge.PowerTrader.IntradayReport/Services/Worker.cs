using Microsoft.Extensions.Options;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppInfraReportSettings _appInfraReportSettings;
        private readonly IIntraReport _intraReport;
        
        public Worker(IOptions<AppInfraReportSettings> appInfraReportSettings, ILogger<Worker> logger , IIntraReport intraReport)
        {
            _logger = logger;
            _appInfraReportSettings = appInfraReportSettings.Value;
            _intraReport= intraReport;
        }

        
        /// <summary>
        /// Execute the GenerateIntraReport based on the ScheduledRunIntervalInMinutes
        /// ScheduledRunIntervalInMinutes configured Minimum one minutes and maximum 24 hours 
        /// </summary>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker process initiated ScheduledRun for every : {_appInfraReportSettings.ScheduledRunIntervalInMinutes} Minutes. Process started at: {DateTimeOffset.Now}", DateTimeOffset.Now);                
                // Maximum ScheduledRunInterval controlled for 24 hours i.e 60 *24 = 1440 minute
                if (Convert.ToInt16(_appInfraReportSettings.ScheduledRunIntervalInMinutes) >1 && Convert.ToInt16(_appInfraReportSettings.ScheduledRunIntervalInMinutes) <= 1440)
                {
                    var _reportStatus = await _intraReport.GenerateIntraReport();

                    //converting minute to milliseconds ex: 1 minute = 60000 milliseconds
                    await Task.Delay(Convert.ToInt16(_appInfraReportSettings.ScheduledRunIntervalInMinutes) * 60000, stoppingToken);                                        
                }
                else
                {
                    _logger.LogInformation("Invalid ScheduledRunInterval, Please check the configuration values", DateTimeOffset.Now);
                    await Task.FromException(new InvalidOperationException("Invalid ScheduledRunInterval and Appsettings.json config values not defined appropriately!, Please check the configuration values"));

                }

            }            
        }


 
    }
}