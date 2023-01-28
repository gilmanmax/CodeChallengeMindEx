using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using CodeChallenge.Models;
using CodeChallenge.Repositories;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void Test_Do_Not_Add_Duplicate_PK()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "dupeattempt101",
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                Employee = employee,
                Salary = 60000.00m
            };

            //first add should be allowed so as there are no dupes

            var requestContent = new JsonSerialization().ToJson(compensation);
            Task<HttpResponseMessage> postRequestTask = Task.Run(() => _httpClient.PostAsync("api/compensation",
   new StringContent(requestContent, Encoding.UTF8, "application/json")));
            postRequestTask.Wait();

            Assert.AreEqual(HttpStatusCode.Created, postRequestTask.Result.StatusCode);

            //it should not allow the second add

             postRequestTask = Task.Run(() => _httpClient.PostAsync("api/compensation",
   new StringContent(requestContent, Encoding.UTF8, "application/json")));
            postRequestTask.Wait();

            Assert.AreEqual(HttpStatusCode.InternalServerError, postRequestTask.Result.StatusCode);

        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "1",
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                Employee = employee,
                Salary = 60000.00m
            };


            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
            Assert.AreEqual(employee.Department, newCompensation.Employee.Department);
            Assert.AreEqual(employee.Position, newCompensation.Employee.Position);
        }

        [TestMethod]
        public void GetCompensation_By_Employee_With_Id()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "999",
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };
            var compensation = new Compensation()
            {
                EffectiveDate = new DateTime(2022, 01, 01),
                Employee = employee,
                Salary = 60000.00m,
                ID = 999
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            Task<HttpResponseMessage> postRequestTask = Task.Run(() => _httpClient.PostAsync("api/compensation",
   new StringContent(requestContent, Encoding.UTF8, "application/json")));
            postRequestTask.Wait();
            string id = employee.EmployeeId;
            Task<HttpResponseMessage> getRequestTask = Task.Run(() => _httpClient.GetAsync($"api/compensation/{id}"));
            getRequestTask.Wait();
            var response = getRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation);
            Assert.IsNotNull(newCompensation.Employee);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.ID, newCompensation.ID);
            Assert.AreEqual(compensation.Employee.FirstName, compensation.Employee.FirstName);
            Assert.AreEqual(compensation.Employee.LastName, compensation.Employee.LastName);
            Assert.AreEqual(compensation.Employee.Department, compensation.Employee.Department);

        }
    }
}
