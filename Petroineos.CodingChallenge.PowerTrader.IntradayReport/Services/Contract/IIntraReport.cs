using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineos.CodingChallenge.PowerTrader.IntradayReport.Services
{
    public interface IIntraReport
    {
        Task<bool> GenerateIntraReport();
    }
}
