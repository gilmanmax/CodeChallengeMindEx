using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using CodeChallenge.Models;
using CodeChallenge.Repositories;
using CodeChallenge.Data;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;
        private const string JOHN_LENNON_EMPLOYEE_ID = "16a596ae-edd3-4847-99fe-c4518e82c86f";
        private static EmployeeContext dbContext;
        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
            var builder = new DbContextOptionsBuilder<EmployeeContext>();
            dbContext = new EmployeeContext(new DbContextOptionsBuilder<EmployeeContext>().UseInMemoryDatabase("EmployeeDB").Options);

        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        /// <summary>
        /// Tests to make sure duplicate PKS are not supported and repository persists.
        /// Adds a 
        /// </summary>
        [TestMethod]
        public void Test_Do_Not_Add_Duplicate_PK()
        {
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                EmployeeId = JOHN_LENNON_EMPLOYEE_ID,
                Salary = 60000.00m
            };

            //first add should be allowed so as there are no dupes

            dbContext.Compensations.Add(compensation);
            dbContext.SaveChanges();

            var requestContent = new JsonSerialization().ToJson(compensation);
            Task<HttpResponseMessage> postRequestTask = Task.Run(() => _httpClient.PostAsync("api/compensation",
   new StringContent(requestContent, Encoding.UTF8, "application/json")));
            postRequestTask.Wait();

            Assert.AreEqual(HttpStatusCode.InternalServerError, postRequestTask.Result.StatusCode);

        }
        /// <summary>
        /// Tests api/compensation post
        /// Once in this method - calling the context should return a relationship between compensation and employee
        /// </summary>
        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                EmployeeId = JOHN_LENNON_EMPLOYEE_ID,
                Salary = 60000.00m
            };


            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = Task.Run(()=> _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json")));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);

            //load from repo
            var comp = dbContext.Compensations.FirstOrDefault(p => p.EmployeeId == JOHN_LENNON_EMPLOYEE_ID);
            var emp = dbContext.Employees.FirstOrDefault(p => p.EmployeeId == JOHN_LENNON_EMPLOYEE_ID);
            Assert.IsNotNull(comp);
            Assert.IsNotNull(comp.Employee);

            Assert.IsNotNull(emp);
            Assert.AreEqual(comp.Salary, newCompensation.Salary);
            Assert.AreEqual(comp.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(emp.Department, comp.Employee.Department);
            Assert.AreEqual(emp.Position, comp.Employee.Position);
        }

        /// <summary>
        /// Tests the api/compensation/employeeid/{employeeid} method.
        /// Adding gets done manually so only failure happens in get method.
        /// </summary>

        [TestMethod]
        public void GetCompensation_By_Employee_With_Id()
        {
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                Salary = 60000.00m,
                CompensationId = 999,
                EmployeeId = JOHN_LENNON_EMPLOYEE_ID
            };

            //

            dbContext.Compensations.Add(compensation);
            dbContext.SaveChanges();

            Task<HttpResponseMessage> getRequestTask = Task.Run(() => _httpClient.GetAsync($"api/compensation/employeeid/{JOHN_LENNON_EMPLOYEE_ID}"));
            getRequestTask.Wait();
            var response = getRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);

            var emp = dbContext.Employees.FirstOrDefault(p => p.EmployeeId == JOHN_LENNON_EMPLOYEE_ID);

            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.CompensationId, newCompensation.CompensationId);
            Assert.AreEqual(emp.FirstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(emp.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(emp.Department, newCompensation.Employee.Department);

        }
    }
}
