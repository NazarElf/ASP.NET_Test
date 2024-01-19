using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ASP.NET_Test.Database
{
    public enum StringTypes
    {
        FULL_NAME,
        ADDRESS,
        PHONE_NUMBER,
    }
    public class SQLiteStringFilter : IFilter
    {
        private const string FULL_NAME_COLUMN_NAME = "FullName";
        private const string ADDRESS_COLUMN_NAME = "Address";
        private const string PHONE_NUMBER_COLUMN_NAME = "PhoneNumber";
        private string columnName;
        private List<string> _filters;
        public string Value { get; set; }
        public SQLiteStringFilter(StringTypes type, List<string> anotherFilter = null)
        {
            _filters = anotherFilter ?? new List<string>();
            columnName = type switch
            {
                StringTypes.FULL_NAME => FULL_NAME_COLUMN_NAME,
                StringTypes.ADDRESS => ADDRESS_COLUMN_NAME,
                StringTypes.PHONE_NUMBER => PHONE_NUMBER_COLUMN_NAME,
                _ => throw new Exception("Unavailable column type"),
            };
        }

        public StringBuilder GetFilter()
        {
            return new StringBuilder().Append(" WHERE ").AppendJoin(" AND ", GetPartialFilter());
        }

        public List<string> GetPartialFilter()
        {
            if (Value.Contains('\''))
                throw new Exception("Attemtion to SQL Inject");
            if (Value != null && Value.Trim() != "")
            {
                _filters.Add($"{columnName} LIKE '%{Value}%'");
            }
            return _filters;
        }
    }
}
