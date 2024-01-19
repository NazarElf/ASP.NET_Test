using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using ASP.NET_Test.Models;
using System.Text;
using System.Configuration;

namespace ASP.NET_Test.Database
{
    public class SQLiteDbConnection : IDbConnection
    {
        private const string DEFAULT_EMPLOYEES_QUERY = "SELECT * FROM Employees ";
        private const string DEFAULT_DEPARTMENT_QUERY = "SELECT * FROM Departments";
        private const string DEFAULT_POSITION_QUERY = "SELECT * FROM Position";
        private const string DEFAULT_SUM_QUERY = "SELECT SUM(Salary) FROM Employees ";
        private readonly string defaultPath = ConfigurationManager.AppSettings["message"].ToString();
        private SQLiteConnection connection;
        /// <summary>
        /// Connects to Sqlite database using specified path to file
        /// </summary>
        /// <param name="path">Path to database file</param>
        public SQLiteDbConnection(string path = null)
        {
            var dbPath = path ?? defaultPath;
            connection = new SQLiteConnection("DataSource=" + dbPath);
        }

        public IEnumerable<DepartmentModel> GetDepartments()
        {
            var result = new List<DepartmentModel>();
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = DEFAULT_DEPARTMENT_QUERY;
                using (var reader = command.ExecuteReader())
                {
                    var canRead = reader.Read();
                    while(canRead)
                    {
                        var id = reader.GetInt64(0);
                        var departmentName = reader.GetString(1);
                        canRead = reader.Read();
                        result.Add(new DepartmentModel() { ID = id, Name = departmentName });
                    }
                }
            }
            catch(SQLiteException ex)
            {
                //log
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public IEnumerable<EmployeeModel> GetEmployees(IFilter filters)
        {
            var result = new List<EmployeeModel>();
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = GetEmployeesQuery(filters);

                using (var reader = command.ExecuteReader())
                {
                    var canRead = reader.Read();
                    while(canRead)
                    {
                        var id = reader.GetInt64(0);
                        var fullName = reader.GetString(1);
                        var addr = reader.GetString(2);
                        var phNum = reader.GetString(3);
                        var bday = reader.GetDateTime(4);
                        var hrDate = reader.GetDateTime(5);
                        var salary = reader.GetDecimal(6);
                        var dpId = reader.GetInt64(7);
                        var posId = reader.GetInt64(8);
                        result.Add(new EmployeeModel()
                        {
                            ID = id,
                            FullName = fullName,
                            Address = addr,
                            PhoneNumber = phNum,
                            Birthday = bday,
                            HiringDate = hrDate,
                            Salary = salary,
                            DepartmentID = dpId,
                            PositionID = posId
                        });
                        canRead = reader.Read();
                    }
                }
            }
            catch(SQLiteException ex)
            {
                //Here must be log
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public IEnumerable<PositionModel> GetPositions()
        {
            var result = new List<PositionModel>();
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = DEFAULT_POSITION_QUERY;
                using (var reader = command.ExecuteReader())
                {
                    var canRead = reader.Read();
                    while (canRead)
                    {
                        var id = reader.GetInt64(0);
                        var departmentName = reader.GetString(1);
                        canRead = reader.Read();
                        result.Add(new PositionModel() { ID = id, Name = departmentName });
                    }
                }
            }
            catch (SQLiteException ex)
            {
                //log
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public void UpdateEmployee(EmployeeModel employee)
        {
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"UPDATE Employees SET " +
                    $"FullName = '{employee.FullName}', " +
                    $"Address = '{employee.Address}', " +
                    $"PhoneNumber = '{employee.PhoneNumber}', " +
                    $"Birthday = '{employee.Birthday.ToString("yyyy-MM-dd")}', " +
                    $"HiringDate = '{employee.HiringDate.ToString("yyyy-MM-dd")}', " +
                    $"Salary = {employee.Salary.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture)}, " +
                    $"Department= {employee.DepartmentID}," +
                    $"Position = {employee.PositionID} " +
                    $"WHERE ID = {employee.ID}";
                command.ExecuteNonQuery();
            }
            catch(SQLiteException ex)
            {
                //log
            }
            finally
            {
                connection.Close();
            }
        }

        public decimal GetMaximumSalary()
        {
            var result = -1.0M;
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT MAX(Salary) FROM Employees";
                using(var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetDecimal(0);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                //Some logs
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public double GetPayroll(IFilter filter)
        {
            double? result = .0d;
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = GetPayrollQuery(filter);
                using(var reader = command.ExecuteReader())
                {
                    var canRead = reader.Read();
                    if(canRead)
                    {
                        result = reader.GetDouble(0);
                    }
                }
            }
            catch (Exception ex)
            {
                //make logs
            }
            finally
            {
                connection.Close();
            }
            return result ?? .0d;
        }
        public string GetAbout()
        {
            string res = null;
            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Data FROM About";
                using(var reader = command.ExecuteReader())
                {
                    if(reader.Read())
                    {
                        res = reader.GetString(0);
                    }
                }
            }
            catch(SQLiteException ex)
            {
                //logs
            }
            finally
            { connection.Close(); }
            return res;
        }

        private string GetEmployeesQuery(IFilter filter)
        {
            if (filter != null)
                return new StringBuilder().Append(DEFAULT_EMPLOYEES_QUERY).Append(filter.GetFilter()).ToString();
            return DEFAULT_EMPLOYEES_QUERY;
        }
        private string GetPayrollQuery(IFilter filter)
        {
            if (filter != null)
                return new StringBuilder().Append(DEFAULT_SUM_QUERY).Append(filter.GetFilter()).ToString();
            return DEFAULT_SUM_QUERY;
        }
    }
}
