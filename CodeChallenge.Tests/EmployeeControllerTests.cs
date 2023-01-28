
using System.Net;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
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

        /// <summary>
        /// Test case to get reports:
        /// Structure is:
        ///        SR
        ///        |
        ///  MID1  MID2  MID 3
        ///  |      |     |
        ///  10Jrs  10Jrs  None
        /// </summary>
        [TestMethod]
        public void TestEmployee_Get_Number_Of_Reports()
        {
            Employee senior = PrepareEmployee();
            //structr
            ReportingStructure srStructure = new ReportingStructure(senior);

            ReportingStructure mid1Structure = new ReportingStructure(senior.DirectReports.FirstOrDefault(p => p.EmployeeId == "EM1"));

            ReportingStructure mid3Structure = new ReportingStructure(senior.DirectReports.FirstOrDefault(p => p.EmployeeId == "EM3"));
            Assert.IsNotNull(mid3Structure.Employee);
            //sr should have 23 reports
            Assert.AreEqual(23, srStructure.NumberOfReports);
            //mid 1 should have 10 rpeorts
            Assert.AreEqual(10, mid1Structure.NumberOfReports);
            //mid 3 should have not any reports
            Assert.AreEqual(0, mid3Structure.NumberOfReports);
        }
        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
            Assert.AreEqual(2, employee.DirectReports.Count);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void TestStructureReturnsOkWithCorrectResult()
        {

            string id = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var getTask = _httpClient.GetAsync($"api/employee/GetDirectReports/{id}");
            var response = getTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var responseObj = response.DeserializeContent<ReportingStructure>();
            
            //should match
            Assert.IsInstanceOfType(responseObj, typeof(ReportingStructure));

            Assert.AreEqual(4, responseObj.NumberOfReports);

        }
        /// <summary>
        /// Generates a senior employee, who has 3 mids. Mids 1 and 2 have 10 juniors and the 3rd mid doesn't have any reports.
        /// </summary>
        /// <returns></returns>
        public Employee PrepareEmployee()
        {
            List<Employee> juniors1 = new();
            for (int i = 1; i <= 10; i++)
            {
                juniors1.Add(new Employee
                {
                    FirstName = "junior1first" + i,
                    LastName = "juniorlast1" + i,
                    Department = "junior programmer frontend",
                    EmployeeId = "EJ1" + i
                });
            }
            List<Employee> juniors2 = new();
            for (int i = 1; i <= 10; i++)
            {
                juniors2.Add(new Employee
                {
                    FirstName = "junior2first" + i,
                    LastName = "juniorlast2" + i,
                    Department = "junior programmer backend",
                    EmployeeId = "EJ2" + i
                });
            }
            List<Employee> mids = new();
            for (int i = 1; i <= 3; i++)
            {
                mids.Add(new Employee
                {
                    FirstName = "mid2first" + i,
                    LastName = "mid2last" + i,
                    Department = "mid department",
                    EmployeeId = "EM" + i,
                    DirectReports = i == 1 ? juniors1 : i == 2 ? juniors2 : new List<Employee>()
                });
            };

            Employee senior = new()
            {
                FirstName = "Senior First",
                LastName = "First",
                Department = "Senior Dept",
                EmployeeId = "ES1",
                DirectReports = mids
            };
            return senior;
        }
    }
}
