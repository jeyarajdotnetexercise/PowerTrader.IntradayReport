using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Model;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services
{
    public interface ICSVReportService
    {
        void GenerateIntraReport<T>(string path, List<T> intraReportModel);
    }
}
