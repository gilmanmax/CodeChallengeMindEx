using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public string EmployeeId { get; set; }
        public ReportingStructure(Employee employee)
        {
            Employee = employee;
        }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        /// <summary>
        /// Number of people who report to employee whether directly or through someone else.
        /// </summary>
        public int NumberOfReports 
        { 
            get
            {
                if (Employee?.DirectReports == null) { return 0; }
                return Employee.DirectReports.Count + Employee.DirectReports.SelectMany(p => p.DirectReports).Count();

            }
        }
    }
}
