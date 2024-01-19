
using ASP.NET_Test.Models;
using System.Collections.Generic;

namespace ASP.NET_Test.Database
{
    //if we going to change Db Type, we have to have an ability to fast adapt to it
    interface IDbConnection
    {
        IEnumerable<EmployeeModel> GetEmployees(IFilter query);
        IEnumerable<PositionModel> GetPositions();
        IEnumerable<DepartmentModel> GetDepartments();
        double GetPayroll(IFilter filter);
        string GetAbout();
        void UpdateEmployee(EmployeeModel employee);
    }
}
