using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Test.Models
{
    public enum TypesOfData
    {
        AVERAGE,
        MAX,
        MIN,
        SUM
    }
    public class PayrollMainViewModel
    {
        public string Department { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public TypesOfData? DataType { get; set; }
    }
}
