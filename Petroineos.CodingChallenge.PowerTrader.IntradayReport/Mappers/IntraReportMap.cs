using CsvHelper.Configuration;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Model;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Mappers
{
    public sealed class IntraReportMap : ClassMap<IntraReportView>
    {
        public IntraReportMap()
        {
            Map(m => m.LocalTime).Name(ReportConstants.LocalTime);
            Map(m => m.Volume).Name(ReportConstants.Volume);            
        }
    }
}
