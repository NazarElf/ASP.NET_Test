using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.NET_Test.Models
{
    public class EmployeeModel
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
        public decimal Salary { get; set; }
        public long DepartmentID { get; set; }
        public long PositionID { get; set; }
    }
}
