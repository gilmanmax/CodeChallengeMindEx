using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService)
        {
            _logger = logger;
            _compensationService = compensationService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            if (compensation == null || compensation.Employee == null) 
            {
                return BadRequest("Cannot supply null compensation or employee record."); 
            }
            _logger.LogDebug($"Received employee create request for '{compensation.Employee.FirstName} {compensation.Employee.LastName}'");

            _compensationService.Create(compensation);

            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId }, compensation);
        }

        [HttpGet("{id}",Name ="getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(string id)
        {
            _logger.LogDebug($"Received compensation get request for employee with '{id}'");

            var employee = _compensationService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }
    }
}
