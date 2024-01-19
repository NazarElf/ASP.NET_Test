using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASP.NET_Test.Models
{
    public class EmployeeViewModel
    {
        public long ID { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        public string Address { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Hiring Date")]
        public DateTime HiringDate { get; set; }
        public string Salary { get; set; }
        public long DepartmentID { get; set; }
        public long PositionID { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }

        public IEnumerable<SelectListItem> DepartmentsList { get; set; }
        public IEnumerable<SelectListItem> PositionsList { get; set; }
    }
}
