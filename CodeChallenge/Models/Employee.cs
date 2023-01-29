using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Models
{
    /// <summary>
    /// Represents an Employee at a company.
    /// </summary>
    public class Employee
    {
        public Employee() 
        {
            DirectReports = new List<Employee>();
        }
        public String EmployeeId { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Position { get; set; }
        public String Department { get; set; }
        /// <summary>
        /// The people who directly report to this employee.
        /// </summary>
        public List<Employee> DirectReports { get; set; }
        public Compensation Compensation { get; set; }

    }
}
