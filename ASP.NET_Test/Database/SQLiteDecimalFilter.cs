using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ASP.NET_Test.Database
{
    public class SQLiteDecimalFilter : IFilter
    {
        private static string COLUMN_NAME = "Salary";
        public decimal? MoreThan { get; set; }
        public decimal? LessThan { get; set; }
        private List<string> _filters;
        public SQLiteDecimalFilter(List<string> anotherFilter = null)
        {
            _filters = anotherFilter ?? new List<string>();
        }

        public List<string> GetPartialFilter()
        {
            if (MoreThan.HasValue)
                _filters.Add($"{COLUMN_NAME} >= {MoreThan}");
            if (LessThan.HasValue)
                _filters.Add($"{COLUMN_NAME} <= {LessThan}");
            return _filters;
        }

        public StringBuilder GetFilter()
        {
            return new StringBuilder().Append(" WHERE ").AppendJoin(" AND ", GetPartialFilter());
        }
    }
}
