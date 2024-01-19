using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_Test.Database
{
    public enum DateTypes
    {
        BIRTHDAY,
        HIRING_DATE,
    }
    public class SQLiteDateTimeFilter : IDateTimeFilter
    {
        private const string BIRTHDAY_COLUMN_NAME = "Birthday";
        private const string HIRING_DATE_COLUMN_NAME = "HiringDate";
        private bool _isExactSet = false;
        private bool _exact;
        private DateTime? _before;
        private DateTime? _after;
        private List<int> _days;
        private List<int> _months;
        private List<int> _years;
        private string _columnName;
        private List<string> _filters;
        private StringBuilder _resultFilter;
        public SQLiteDateTimeFilter(DateTypes type, List<string> anotherFilter = null)
        {
            _filters = anotherFilter ?? new List<string>();
            _columnName = type switch {
                DateTypes.BIRTHDAY => BIRTHDAY_COLUMN_NAME,
                DateTypes.HIRING_DATE => HIRING_DATE_COLUMN_NAME,
                _ => throw new Exception("No such date type")
            };
        }
        public bool Exact
        {
            get => _exact;
            set
            {
                if (_isExactSet && _exact != value)
                {
                    throw new Exception("Cannot change exact setting of filter. Filter can be either exact Date, or range of Dates");
                }
                _exact = value;
                _isExactSet = true;
            }
        }
        public DateTime? Before
        {
            get => _before;
            set
            {
                Exact = false;
                _before = value;
            }

        }
        public DateTime? After
        {
            get => _after;
            set
            {
                Exact = false;
                _after = value;
            }
        }
        public List<int> Days
        {
            get => _days;
            set
            {
                Exact = true;
                _days = value;
            }
        }
        public List<int> Months
        {
            get => _months;
            set
            {
                Exact = true;
                _months = value;
            }
        }
        public List<int> Years
        {
            get => _years;
            set
            {
                Exact = true;
                _years = value;
            }
        }

        public StringBuilder GetFilter()
        {
            return _resultFilter ?? (_resultFilter = new StringBuilder().Append(" WHERE ").AppendJoin(" AND ", GetPartialFilter()));
        }

        public List<string> GetPartialFilter()
        {

            if (_exact)
            {
                if (_days != null && _days.Count > 0)
                {
                    foreach (var day in _days)
                    {
                        _filters.Add($"strftime('%d', {_columnName})='{day.ToString("00")}'");
                    }
                }
                if (_months != null && _months.Count > 0)
                {
                    foreach (var month in _months)
                    {
                        _filters.Add($"strftime('%m', {_columnName})='{month.ToString("00")}'");
                    }
                }
                if (_years != null && _years.Count > 0)
                {
                    foreach (var year in _years)
                    {
                        _filters.Add($"strftime('%y', {_columnName})='{year.ToString("00")}'");
                    }
                }
                return _filters;
            }
            if (_before.HasValue)
            {
                _filters.Add($"{_columnName} <= '{_before.Value.ToString("yyyy-MM-dd")}'");
            }
            if (_after.HasValue)
            {
                _filters.Add($"{_columnName} >= '{_after.Value.ToString("yyyy-MM-dd")}'");
            }
            return _filters;

        }
    }
}
