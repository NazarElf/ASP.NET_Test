using ASP.NET_Test.Database;
using ASP.NET_Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASP.NET_Test.Controllers
{
    public class PersonnelController : Controller
    {
        public IActionResult Index(FilteredListViewModel filters)
        {
            var res = View();
            var sqlFilters = CreateFilters(filters);

            var sqlite = new SQLiteDbConnection();
            var positions = sqlite.GetPositions();
            var departments = sqlite.GetDepartments();
            var models = sqlite.GetEmployees(sqlFilters);
            var sum = sqlite.GetPayroll(sqlFilters);
            ViewBag.Sum = sum.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            var viewModels = models.Select(model => new EmployeeViewModel()
            {
                ID = model.ID,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Birthday = model.Birthday,
                HiringDate = model.HiringDate,
                Salary = model.Salary.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                Department = departments.First(dep => dep.ID == model.DepartmentID).Name,
                Position = positions.First(pos => pos.ID == model.PositionID).Name
            }).ToList();

            var model = new FilteredListViewModel()
            {
                Data = viewModels,
                BirthdayMonthList = Enumerable.Range(1, 12).Select(i => (i,new DateTime(2000, i, 1).ToString("MMMM"))).Select(pair =>
                new SelectListItem(pair.Item2,pair.i.ToString("00"))),
                MaximumSalary = sqlite.GetMaximumSalary().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                DepartmetnsList = new SelectList(departments, "ID","Name"),
                PositionsList = new SelectList(positions, "ID", "Name"),
                SessionID = Guid.NewGuid().ToString()
            };
            HttpContext.Session.SetString(model.SessionID, 
                JsonConvert.SerializeObject(new Tuple<List<EmployeeViewModel>, double>(viewModels, sum)));

            return View(model);
        }

        public IActionResult Person(int id)
        {
            var res = View();

            var sqlite = new SQLiteDbConnection();
            var queryResult = sqlite.GetEmployees(new SQLiteIntegerListFilter(IntegerListTypes.ID) { Values = new List<long> { id } });
            if (queryResult.Count() == 0)
                return null;

            var positions = sqlite.GetPositions();
            var departments = sqlite.GetDepartments();
            var model = queryResult.First();
            var viewModel = new EmployeeViewModel()
            {
                ID = model.ID,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Birthday = model.Birthday,
                HiringDate = model.HiringDate,
                Salary = model.Salary.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                DepartmentID = model.DepartmentID,
                PositionID = model.PositionID,
                Department = departments.First(dep => dep.ID == model.DepartmentID).Name,
                Position = positions.First(pos => pos.ID == model.PositionID).Name
            };
            //var a = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(departments, "", "", queryResult[0].Department);
            //var positionsList = new SelectList(positions, "Position", "Position", queryResult[0].Position);
            var depsSelList = new SelectList(departments, "ID", "Name", model.DepartmentID);
            var possSelList = new SelectList(positions, "ID", "Name", model.PositionID);

            ViewBag.DepartmentID = depsSelList;
            ViewBag.PositionID = possSelList;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Person(EmployeeViewModel employee)
        {
            var modelToDb = new EmployeeModel()
            {
                ID = employee.ID,
                FullName = employee.FullName,
                Address = employee.Address,
                PhoneNumber = employee.PhoneNumber,
                Birthday = employee.Birthday,
                HiringDate = employee.HiringDate,
                Salary = decimal.Parse(employee.Salary,System.Globalization.CultureInfo.InvariantCulture),
                DepartmentID = employee.DepartmentID,
                PositionID = employee.PositionID,
            };

            var sqlite = new SQLiteDbConnection();
            sqlite.UpdateEmployee(modelToDb);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Export(string SessionID)
        {
            var strBuilder = new StringBuilder();
            var (filters, sum) = JsonConvert.DeserializeObject<Tuple<List<EmployeeViewModel>, double>>(HttpContext.Session.GetString(SessionID));

            foreach (var dataRow in filters)
            {
                strBuilder.Append(dataRow.FullName);
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.Address);
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.PhoneNumber);
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.Birthday.ToString("yyyy-MM-dd"));
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.HiringDate.ToString("yyyy-MM-dd"));
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.Salary);
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.Department);
                strBuilder.Append('\t');
                strBuilder.Append(dataRow.Position);
                strBuilder.Append('\n');
            }
            strBuilder.Append("\nSum of salaries:\t");
            strBuilder.Append(sum);
            return File(Encoding.UTF8.GetBytes(strBuilder.ToString()), "text/plain", "records.txt");
        }

        private IFilter CreateFilters(FilteredListViewModel filters)
        {
            IFilter filter = null;
            if(filters.NameFilter != null)
            {
                filter = new SQLiteStringFilter(StringTypes.FULL_NAME) { Value = filters.NameFilter };
            }
            if(filters.AddressFilter != null)
            {
                filter = new SQLiteStringFilter(StringTypes.ADDRESS, filter?.GetPartialFilter())
                { Value = filters.AddressFilter };
            }
            if(filters.PhoneNumberFilter != null)
            {
                filter = new SQLiteStringFilter(StringTypes.PHONE_NUMBER, filter?.GetPartialFilter())
                { Value = filters.PhoneNumberFilter };
            }
            if(filters.BirthdayMonthFilter != null)
            {
                filter = new SQLiteDateTimeFilter(DateTypes.BIRTHDAY, filter?.GetPartialFilter())
                { Months = new List<int>() { int.Parse(filters.BirthdayMonthFilter) } };
            }
            if(filters.HireDateFromFilter != null || filters.HireDateToFilter != null)
            {
                filter = new SQLiteDateTimeFilter(DateTypes.HIRING_DATE, filter?.GetPartialFilter())
                {
                    Before = filters.HireDateToFilter,
                    After = filters.HireDateFromFilter
                };
            }
            if(filters.SalaryFromFilter != null || filters.SalaryToFilter != null)
            {
                filter = new SQLiteDecimalFilter(filter?.GetPartialFilter())
                {
                    LessThan = decimal.Parse(filters.SalaryToFilter, System.Globalization.CultureInfo.InvariantCulture),
                    MoreThan = decimal.Parse(filters.SalaryFromFilter, System.Globalization.CultureInfo.InvariantCulture)
                };
            }
            if(filters.DepartmentIDFilter.HasValue)
            {
                filter = new SQLiteIntegerListFilter(IntegerListTypes.DEPARTMENT, filter?.GetPartialFilter())
                {
                    Values = new List<long>() { filters.DepartmentIDFilter.Value }
                };
            }
            if(filters.PositionIDFilter.HasValue)
            {
                filter = new SQLiteIntegerListFilter(IntegerListTypes.POSITION, filter?.GetPartialFilter())
                {
                    Values = new List<long>() { filters.PositionIDFilter.Value }
                };
            }

            return filter;

        }
    }
}
