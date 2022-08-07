using CsvHelper.Configuration.Attributes;
using Petroineos.CodingChallenge.PowerTrader.IntradayReport.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Model
{
    public class IntraReportView
    {
        [Name(ReportConstants.LocalTime)]
        public string? LocalTime { get; set; }

        [Name(ReportConstants.Volume)]
        public string? Volume { get; set; }        
    }
}
