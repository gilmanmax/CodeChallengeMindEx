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
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext compensationContext)
        {
            _employeeContext = compensationContext;
            _logger = logger;
        }
        /// <summary>
        /// Adds new record to compensation context
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        public Compensation Add(Compensation comp)
        {
            _employeeContext.Compensations.Add(comp);
            return comp;
        }
        /// <summary>
        /// Looks up compensation by employee ID
        /// </summary>
        public Compensation GetByEmployeeId(string empID)
        {
            var compensation = _employeeContext.Compensations.Include(p=>p.Employee).SingleOrDefault(e => e.EmployeeId == empID);
            return compensation;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
