using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using CodeChallenge.Exceptions;

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
        /// <summary>
        /// HTTP POST api/compensation
        /// </summary>
        /// <param name="compensation">Compensation object to post. </param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation compensation)
        {
            try
            {
                _logger.LogDebug($"Received compensation create request for employee is  '{compensation.EmployeeId}'");
                if (compensation == null)
                {
                    return BadRequest("Cannot supply null compensation.");
                }
                _compensationService.Create(compensation);

                return CreatedAtRoute("employeeid", new { employeeid = compensation.EmployeeId }, compensation);
            }
            catch (EmployeeNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500);
            }
        }
        /// <summary>
        /// GET {api/compensation/employee/16a596ae-edd3-4847-99fe-c4518e82c86f
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{employeeid}", Name = "employeeid")]
        [Route("employeeid/{employeeid}")]
        public IActionResult GetCompensationByEmployeeId(string employeeid)
        {
            _logger.LogDebug($"Received compensation get request for employee with '{employeeid}'");

            var comp = _compensationService.GetById(employeeid);


            return Ok(comp);
        }
    }
}
