using System.Linq;

namespace CodeChallenge.Models
{
    public class ReportingStructure
    {
        public ReportingStructure(Employee employee)
        {
            Employee = employee;
        }

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
