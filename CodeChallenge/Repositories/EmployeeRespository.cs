using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        /// <summary>
        /// This DOES NOT eagerly load Direct Reports
        /// </summary>
        public Employee GetById(string id)
        {            
            var employee =  _employeeContext.Employees.Include(p=>p.Compensation).SingleOrDefault(e => e.EmployeeId == id);
            return employee;
        }

        /// <summary>
        /// Needed to load all Direct Reports.
        ///
        public Employee GetEmployeeByIdEagerly(string id)
        {
            var employee = _employeeContext.Employees.Include(p => p.DirectReports).Include(p=>p.Compensation).AsEnumerable().SingleOrDefault(p => p.EmployeeId == id);
            LoadDirectReportsForEmployee(employee);
            return employee;
        }
        
        private void LoadDirectReportsForEmployee(Employee employee)
        {
            if (employee != null)
            {
                var directReports = employee.DirectReports;
                if (directReports != null && directReports.Count > 0)
                {
                    foreach(var directReport in directReports)
                    {
                        LoadDirectReportsForEmployee(directReport);
                    }
                }
            }
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }
    }
}
