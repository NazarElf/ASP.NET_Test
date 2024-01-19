using ASP.NET_Test.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Test.Models
{
    public class FilteredListViewModel
    {
        public List<EmployeeViewModel> Data { get; set; }

        public string NameFilter { get; set; }
        public string AddressFilter { get; set; }
        public string PhoneNumberFilter { get; set; }
        public IEnumerable<SelectListItem> BirthdayMonthList { get; set; }
        public string BirthdayMonthFilter { get; set; }
        [DataType(DataType.Date)]
        public DateTime? HireDateFromFilter { get; set; }
        [DataType(DataType.Date)]
        public DateTime? HireDateToFilter { get; set; }
        public string SalaryFromFilter { get; set; }
        public string SalaryToFilter { get; set; }
        public IEnumerable<SelectListItem> DepartmetnsList { get; set; }
        public IEnumerable<SelectListItem> PositionsList { get; set; }
        public long? DepartmentIDFilter { get; set; }
        public long? PositionIDFilter { get; set; }
        public string MaximumSalary { get; set; }
        public string SessionID { get; set; }
    }
}
