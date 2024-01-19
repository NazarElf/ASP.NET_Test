using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_Test.Database
{
    public interface IFilter
    {
        List<string> GetPartialFilter();
        StringBuilder GetFilter();
    }

    public interface IDateTimeFilter : IFilter
    {
        bool Exact { get; set; }
        DateTime? Before { get; set; }
        DateTime? After { get; set; }
        List<int> Days { get; set; }
        List<int> Months { get; set; }
        List<int> Years { get; set; }
    }
}
