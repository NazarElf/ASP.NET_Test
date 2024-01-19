using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_Test.Database
{
    public enum IntegerListTypes
    {
        POSITION,
        DEPARTMENT,
        ID
    }
    public class SQLiteIntegerListFilter : IFilter
    {
        private const string DEPARTMENT_COLUMN_NAME = "Department";
        private const string POSITION_COLUMN_NAME = "Position";
        private const string ID_COLUMN_NAME = "ID";
        private string columnName;
        private List<string> _filters;
        public List<long> Values{ get; set; }

        
        public SQLiteIntegerListFilter(IntegerListTypes type, List<string> anotherFilter = null)
        {
            _filters = anotherFilter ?? new List<string>();
            columnName = type switch
            {
                IntegerListTypes.DEPARTMENT => DEPARTMENT_COLUMN_NAME,
                IntegerListTypes.POSITION => POSITION_COLUMN_NAME,
                IntegerListTypes.ID => ID_COLUMN_NAME,
                _ => throw new Exception("Unavilable type of list"),
            };
        }

        public StringBuilder GetFilter()
        {
            return new StringBuilder().Append(" WHERE ").AppendJoin(" AND ", GetPartialFilter());
        }

        public List<string> GetPartialFilter()
        {
            foreach(var value in Values)
            {
                _filters.Add($"{columnName} = {value}");
            }
            return _filters;
        }
    }
}
