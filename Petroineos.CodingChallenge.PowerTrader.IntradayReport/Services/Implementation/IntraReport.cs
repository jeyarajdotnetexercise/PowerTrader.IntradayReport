using Microsoft.Extensions.Options;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Model;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services;
using Services;
using System.Globalization;
namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport
{
    public class IntraReport : IIntraReport
    {

        private readonly ILogger<Worker> _logger;
        private readonly AppInfraReportSettings _appInfraReportSettings;
        private readonly ICSVReportService _cSVReportService;

        public IntraReport(IOptions<AppInfraReportSettings> appInfraReportSettings, ILogger<Worker> logger, ICSVReportService cSVReportService)
        {
            _logger = logger;
            _appInfraReportSettings = appInfraReportSettings.Value;
            _cSVReportService = cSVReportService;
        }

        /// <summary>
        /// GenerateIntraReport which process the trade data and upload the result into CSV File
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GenerateIntraReport()
        {
            var currentTimeStamp = DateTime.Now;
            var TradeListsResult = new List<PowerPeriod>();
            List<PowerPeriod> BaseTradeData = new List<PowerPeriod>();
            var ReportFileFullPath = $"{_appInfraReportSettings.ReportLocation}{_appInfraReportSettings.ReportFileName}{currentTimeStamp.ToString(_appInfraReportSettings.ReportFileFormat)}.csv";
            var TradeDataLocation = $"{_appInfraReportSettings.TradeDataLocation}Trade";
            PowerService powerService = new PowerService();
            CultureInfo ci = CultureInfo.InvariantCulture;
            List<PowerTrade> TradeLists = new List<PowerTrade>();

            _logger.LogInformation($"Retrieve power trades for the specific date : {currentTimeStamp}", DateTimeOffset.Now);
            try{ 
                TradeLists = (await powerService.GetTradesAsync(currentTimeStamp)).ToList();
                //Retry the GetTradesAsync when powerService failed to provide the Trade data
                if (TradeLists.Count==0)
                {
                    TradeLists = (await powerService.GetTradesAsync(currentTimeStamp)).ToList();
                }
            }
            catch(Exception)
            {
                _logger.LogInformation($"Problem while retriving PowerService trade data : {currentTimeStamp}", DateTimeOffset.Now);
            }
            if (TradeLists != null)
            {
                //PowerService TradeData input feeds successfully uploaded here 
                TradeListsResult = ProcessTradeData(TradeLists, TradeDataLocation);
            }

            _logger.LogInformation("Power trades consolidated in single list", DateTimeOffset.Now);
            //Transform the data into model for writting CSV File.
            var TradeListsResultView = (from l1 in TradeListsResult
                                        select new IntraReportView()
                                        {
                                            LocalTime = GetLocalTime(l1.Period),
                                            Volume = l1.Volume.ToString($"{_appInfraReportSettings.VolumeFormat}")
                                        }).ToList();

            if (File.Exists(ReportFileFullPath))
            {
                File.Delete(ReportFileFullPath);
            }
            _cSVReportService.GenerateIntraReport(ReportFileFullPath, TradeListsResultView.ToList());
            _logger.LogInformation($"Power trades successfully uploaded here : {ReportFileFullPath}", DateTimeOffset.Now);
            return true;
        }

        /// <summary>
        /// Process the trade data provided by the PowerPeriod
        /// </summary>
        /// <returns></returns>
        private List<PowerPeriod> ProcessTradeData(List<PowerTrade> TradeLists, string TradeDataLocation)
        {
            var restTradeDatas = new List<PowerPeriod>();
            var TradeListsResult = new List<PowerPeriod>();
            var BaseTradeData = new List<PowerPeriod>();

            if (TradeLists.Count >= 1)
            {
                BaseTradeData = TradeLists[0].Periods.ToList();
            }
            if (_appInfraReportSettings.IsRecordTradeData)
            {
                _logger.LogInformation($"IsRecordTradeData Flag is enabled.{TradeLists.Count} power trades input feeds received and uploaded here : {_appInfraReportSettings.TradeDataLocation}", DateTimeOffset.Now);
                Array.ForEach(Directory.GetFiles(_appInfraReportSettings.TradeDataLocation), File.Delete);
                SavePowerTradeData(BaseTradeData, TradeDataLocation, 0);
            }

            //When more than one power trade data received from PowerService 
            for (int tradeFileCount = 1; tradeFileCount <= TradeLists.Count - 1; tradeFileCount++)
            {
                restTradeDatas = TradeLists[tradeFileCount].Periods.ToList();
                if (_appInfraReportSettings.IsRecordTradeData)
                {
                    SavePowerTradeData(restTradeDatas, TradeDataLocation, tradeFileCount);
                }
                TradeListsResult = (from l1 in BaseTradeData
                                    join l2 in restTradeDatas on l1.Period equals l2.Period into j
                                    from l2 in j.DefaultIfEmpty()
                                    select new PowerPeriod()
                                    {
                                        Period = l1.Period,
                                        Volume = (l1.Volume + l2.Volume)
                                    }).ToList();

                BaseTradeData = TradeListsResult;
            }

            //Only one power trade data received from PowerService 
            if (TradeLists.Count == 1)
            {
                _logger.LogInformation($"Power trade list returns single result set", DateTimeOffset.Now);
                TradeListsResult = (from l1 in BaseTradeData
                                    select new PowerPeriod()
                                    {
                                        Period = l1.Period,
                                        Volume = (l1.Volume)
                                    }).ToList();
            }
            return TradeListsResult;
        }


        /// <summary>
        /// Power trade data writes into CSV file
        /// </summary>
        /// <returns></returns>
        private void SavePowerTradeData(List<PowerPeriod> BaseTradeData, string TradeDataLocation, int fileIndex)
        {
            //Received power trade data writes into CSV file.
            if (File.Exists($"{TradeDataLocation}{fileIndex}.csv"))
            {
                File.Delete($"{TradeDataLocation}{fileIndex}.csv");
            }
            _cSVReportService.GenerateIntraReport($"{TradeDataLocation}{fileIndex}.csv", BaseTradeData.ToList());
        }

        /// <summary>
        /// Assign the time based on the period
        /// </summary>
        /// <returns></returns>
        private string GetLocalTime(int period)
        {
            if (period == 1)
            {
                return "23:00";
            }
            else if (period == 2)
            {
                return "00:00";
            }
            else if (period == 3)
            {
                return "01:00";
            }
            else if (period == 4)
            {
                return "02:00";
            }
            else if (period == 5)
            {
                return "03:00";
            }
            else if (period == 6)
            {
                return "04:00";
            }
            else if (period == 7)
            {
                return "05:00";
            }
            else if (period == 8)
            {
                return "06:00";
            }
            else if (period == 9)
            {
                return "07:00";
            }
            else if (period == 10)
            {
                return "08:00";
            }
            else if (period == 11)
            {
                return "09:00";
            }
            else if (period == 12)
            {
                return "10:00";
            }
            else if (period == 13)
            {
                return "11:00";
            }
            else if (period == 14)
            {
                return "12:00";
            }
            else if (period == 15)
            {
                return "13:00";
            }
            else if (period == 16)
            {
                return "14:00";
            }
            else if (period == 17)
            {
                return "15:00";
            }
            else if (period == 18)
            {
                return "16:00";
            }
            else if (period == 19)
            {
                return "17:00";
            }
            else if (period == 20)
            {
                return "18:00";
            }
            else if (period == 21)
            {
                return "19:00";
            }
            else if (period == 22)
            {
                return "20:00";
            }
            else if (period == 23)
            {
                return "21:00";
            }

            else
            {
                return "22:00";
            }
        }
    }
}
