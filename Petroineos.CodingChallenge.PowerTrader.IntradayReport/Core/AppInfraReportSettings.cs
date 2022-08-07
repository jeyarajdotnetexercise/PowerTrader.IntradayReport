namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core
{
    public class AppInfraReportSettings
    {
        public string ReportFileFormat { get; set; }
        public string ReportFileName { get; set; }
        public string ReportLocation { get; set; }
        public string ScheduledRunIntervalInMinutes { get; set; }
        public bool IsRecordTradeData { get; set; }
        public string TradeDataLocation { get; set; }

        public string VolumeFormat { get; set; }
    }

}
